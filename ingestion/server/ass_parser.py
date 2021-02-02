import parser_pb2
import parser_pb2_grpc
import io
import pysubs2


class AssParserServicer(parser_pb2_grpc.AssParserServiceServicer):
    def ParseAss(self, request_iterator, context):
        stream = io.StringIO()
        size = 0
        for request in request_iterator:
            stream.write(request.content.decode("utf-8"))
            size += len(request.content)

        print(f"read {size} bytes")
        stream.seek(0)
        doc = pysubs2.SSAFile.from_file(stream)

        for event in doc.events:
            yield parser_pb2.AssParsedResponseDialog(
                lines=event.plaintext.splitlines(keepends=False),
                time=event.start,
                isComment=event.type != "Dialogue",
                style=event.style,
            )
