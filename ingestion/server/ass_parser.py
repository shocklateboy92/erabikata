import parser_pb2
import parser_pb2_grpc
import io


class AssParserServicer(parser_pb2_grpc.AssParserServiceServicer):
    def ParseAss(self, request_iterator, context):
        stream = io.BytesIO()
        for request in request_iterator:
            stream.write(request.content)

        yield parser_pb2.AssParsedResponseDialog(lines=[], style="sdf", time=0)
