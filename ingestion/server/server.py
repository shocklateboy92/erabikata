import logging
from sudachipy import tokenizer, dictionary
import analyzer_pb2
import analyzer_pb2_grpc

import grpc
from concurrent import futures

analyzer = dictionary.Dictionary().create()
modeMap = {
    analyzer_pb2.AnalyzerMode.SudachiA: tokenizer.Tokenizer.SplitMode.A,
    analyzer_pb2.AnalyzerMode.SudachiB: tokenizer.Tokenizer.SplitMode.B,
    analyzer_pb2.AnalyzerMode.SudachiC: tokenizer.Tokenizer.SplitMode.C,
}


class AnalyzerServicer(analyzer_pb2_grpc.AnalyzerServiceServicer):
    def AnalyzeText(self, request, context):
        # logging.info(
        #     f"Received request of length {len(request.text)} for {modeMap[request.mode]}"
        # )
        results = analyzer.tokenize(request.text, modeMap[request.mode])
        return analyzer_pb2.AnalyzedResponse(
            words=[
                analyzer_pb2.AnalyzedWord(
                    baseForm=word.normalized_form(),
                    dictionaryForm=word.dictionary_form(),
                    reading=word.reading_form(),
                    original=word.surface(),
                    partOfSpeech=word.part_of_speech(),
                )
                for word in results
            ]
        )

    def AnalyzeBulk(self, request_iterator, context):
        logging.info("Received bulk request")
        for request in request_iterator:
            yield self.AnalyzeText(request, context)

    def AnalyzeDialogBulk(self, request_iterator, context):
        logging.info("Recived bulk dialog request")
        for request in request_iterator:
            yield analyzer_pb2.AnalyzeDialogResponse(
                lines=[
                    analyzer_pb2.AnalyzeDialogResponse.Line(
                        words=self.AnalyzeText(
                            analyzer_pb2.AnalyzeRequest(
                                text=requestLine, mode=request.mode
                            ),
                            context,
                        ).words
                    )
                    for requestLine in request.lines
                ],
                time=request.time,
                style=request.style,
            )


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=1))
    analyzer_pb2_grpc.add_AnalyzerServiceServicer_to_server(AnalyzerServicer(), server)
    server.add_insecure_port("[::]:5001")
    server.start()
    server.wait_for_termination()


if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    serve()
