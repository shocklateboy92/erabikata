import parser_pb2
import parser_pb2_grpc
import io
import logging
import pysubs2


class AssParserServicer(parser_pb2_grpc.AssParserServiceServicer):
    def ParseAss(self, request_iterator, context):
        logging.info("Recieved ass parse request")
        stream = io.BytesIO()
        size = 0
        for request in request_iterator:
            stream.write(request.content)
            size += len(request.content)

        print(f"read {size} bytes")
        doc = pysubs2.SSAFile.from_file(io.StringIO(stream.getvalue().decode("utf-8")))

        for event in doc.events:
            yield parser_pb2.AssParsedResponseDialog(
                lines=event.plaintext.splitlines(keepends=False),
                time=float(event.start) / 1000,
                isComment=event.type != "Dialogue",
                style=event.style,
            )
