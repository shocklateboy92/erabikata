from analyzer import AnalyzerServicer
from parser import AssParserServicer
import logging
import analyzer_pb2_grpc
import parser_pb2_grpc

import grpc
from concurrent import futures


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=1))
    analyzer_pb2_grpc.add_AnalyzerServiceServicer_to_server(AnalyzerServicer(), server)
    parser_pb2_grpc.add_AssParserServiceServicer_to_server(AssParserServicer(), server)
    server.add_insecure_port("[::]:5001")
    server.start()
    server.wait_for_termination()


if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO)
    serve()
