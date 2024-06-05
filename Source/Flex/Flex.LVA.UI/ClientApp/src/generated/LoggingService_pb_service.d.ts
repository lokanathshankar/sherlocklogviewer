// package: Flex.LVA.Communication.v1
// file: LoggingService.proto

import * as LoggingService_pb from "./LoggingService_pb";
import * as CommonReturnTypes_pb from "./CommonReturnTypes_pb";
import {grpc} from "@improbable-eng/grpc-web";

type LoggingServiceDebug = {
  readonly methodName: string;
  readonly service: typeof LoggingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof LoggingService_pb.LoggingRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type LoggingServiceInfo = {
  readonly methodName: string;
  readonly service: typeof LoggingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof LoggingService_pb.LoggingRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type LoggingServiceError = {
  readonly methodName: string;
  readonly service: typeof LoggingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof LoggingService_pb.LoggingRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type LoggingServiceVerbose = {
  readonly methodName: string;
  readonly service: typeof LoggingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof LoggingService_pb.LoggingRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type LoggingServiceWarn = {
  readonly methodName: string;
  readonly service: typeof LoggingService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof LoggingService_pb.LoggingRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

export class LoggingService {
  static readonly serviceName: string;
  static readonly Debug: LoggingServiceDebug;
  static readonly Info: LoggingServiceInfo;
  static readonly Error: LoggingServiceError;
  static readonly Verbose: LoggingServiceVerbose;
  static readonly Warn: LoggingServiceWarn;
}

export type ServiceError = { message: string, code: number; metadata: grpc.Metadata }
export type Status = { details: string, code: number; metadata: grpc.Metadata }

interface UnaryResponse {
  cancel(): void;
}
interface ResponseStream<T> {
  cancel(): void;
  on(type: 'data', handler: (message: T) => void): ResponseStream<T>;
  on(type: 'end', handler: (status?: Status) => void): ResponseStream<T>;
  on(type: 'status', handler: (status: Status) => void): ResponseStream<T>;
}
interface RequestStream<T> {
  write(message: T): RequestStream<T>;
  end(): void;
  cancel(): void;
  on(type: 'end', handler: (status?: Status) => void): RequestStream<T>;
  on(type: 'status', handler: (status: Status) => void): RequestStream<T>;
}
interface BidirectionalStream<ReqT, ResT> {
  write(message: ReqT): BidirectionalStream<ReqT, ResT>;
  end(): void;
  cancel(): void;
  on(type: 'data', handler: (message: ResT) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'end', handler: (status?: Status) => void): BidirectionalStream<ReqT, ResT>;
  on(type: 'status', handler: (status: Status) => void): BidirectionalStream<ReqT, ResT>;
}

export class LoggingServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  debug(
    requestMessage: LoggingService_pb.LoggingRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  debug(
    requestMessage: LoggingService_pb.LoggingRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  info(
    requestMessage: LoggingService_pb.LoggingRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  info(
    requestMessage: LoggingService_pb.LoggingRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  error(
    requestMessage: LoggingService_pb.LoggingRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  error(
    requestMessage: LoggingService_pb.LoggingRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  verbose(
    requestMessage: LoggingService_pb.LoggingRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  verbose(
    requestMessage: LoggingService_pb.LoggingRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  warn(
    requestMessage: LoggingService_pb.LoggingRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  warn(
    requestMessage: LoggingService_pb.LoggingRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
}

