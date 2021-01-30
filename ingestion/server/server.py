import logging
from sudachipy import tokenizer, dictionary
from generated import analyzer_pb2
from generated import analyzer_pb2_grpc

import grpc
from concurrent import futures

analyzer = dictionary.Dictionary().create()

res = analyzer.tokenize("国家公務員", tokenizer.Tokenizer.SplitMode.A)
results = [{"surface": a.surface(), "dictionary": a.dictionary_form()} for a in res]

class AnalyzerServicer(analyzer_pb2_grpc.AnalyzerServiceServicer):
    def AnalyzeText(self, request, context):
        return analyzer_pb2.AnalyzedResponse(words=results)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    analyzer_pb2_grpc.add_AnalyzerServiceServicer_to_server(AnalyzerServicer(), server)
    server.add_insecure_port("[::]:5000")
    server.start()
    server.wait_for_termination()


if __name__ == "__main__":
    logging.basicConfig()
    serve()
