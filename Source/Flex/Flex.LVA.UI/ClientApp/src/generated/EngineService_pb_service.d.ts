// package: Flex.LVA.Communication.v1
// file: EngineService.proto

import * as EngineService_pb from "./EngineService_pb";
import * as CommonReturnTypes_pb from "./CommonReturnTypes_pb";
import {grpc} from "@improbable-eng/grpc-web";

type EngineServiceGetRawLogs = {
  readonly methodName: string;
  readonly service: typeof EngineService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof EngineService_pb.GetRawLogsRequest;
  readonly responseType: typeof CommonReturnTypes_pb.StringMessage;
};

type EngineServiceGetRawLog = {
  readonly methodName: string;
  readonly service: typeof EngineService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof EngineService_pb.GetRawLogRequest;
  readonly responseType: typeof CommonReturnTypes_pb.StringMessage;
};

type EngineServicePrepareLogs = {
  readonly methodName: string;
  readonly service: typeof EngineService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof CommonReturnTypes_pb.RegistrationData;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type EngineServiceAppendLogChunk = {
  readonly methodName: string;
  readonly service: typeof EngineService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof EngineService_pb.LogChunkRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

type EngineServicePrepareResources = {
  readonly methodName: string;
  readonly service: typeof EngineService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof EngineService_pb.PrepareResourceRequest;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

export class EngineService {
  static readonly serviceName: string;
  static readonly GetRawLogs: EngineServiceGetRawLogs;
  static readonly GetRawLog: EngineServiceGetRawLog;
  static readonly PrepareLogs: EngineServicePrepareLogs;
  static readonly AppendLogChunk: EngineServiceAppendLogChunk;
  static readonly PrepareResources: EngineServicePrepareResources;
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

export class EngineServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  getRawLogs(
    requestMessage: EngineService_pb.GetRawLogsRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  getRawLogs(
    requestMessage: EngineService_pb.GetRawLogsRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  getRawLog(
    requestMessage: EngineService_pb.GetRawLogRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  getRawLog(
    requestMessage: EngineService_pb.GetRawLogRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  prepareLogs(
    requestMessage: CommonReturnTypes_pb.RegistrationData,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  prepareLogs(
    requestMessage: CommonReturnTypes_pb.RegistrationData,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  appendLogChunk(
    requestMessage: EngineService_pb.LogChunkRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  appendLogChunk(
    requestMessage: EngineService_pb.LogChunkRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  prepareResources(
    requestMessage: EngineService_pb.PrepareResourceRequest,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  prepareResources(
    requestMessage: EngineService_pb.PrepareResourceRequest,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
}

