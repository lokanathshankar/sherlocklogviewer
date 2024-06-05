// package: Flex.LVA.Communication.v1
// file: RegistrationService.proto

import * as RegistrationService_pb from "./RegistrationService_pb";
import * as CommonReturnTypes_pb from "./CommonReturnTypes_pb";
import {grpc} from "@improbable-eng/grpc-web";

type RegistrationServiceStopServices = {
  readonly methodName: string;
  readonly service: typeof RegistrationService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof CommonReturnTypes_pb.RegistrationData;
  readonly responseType: typeof CommonReturnTypes_pb.BoolMessage;
};

type RegistrationServiceStartServices = {
  readonly methodName: string;
  readonly service: typeof RegistrationService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof CommonReturnTypes_pb.VoidMessage;
  readonly responseType: typeof CommonReturnTypes_pb.RegistrationData;
};

type RegistrationServiceReadServiceVersion = {
  readonly methodName: string;
  readonly service: typeof RegistrationService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof CommonReturnTypes_pb.VoidMessage;
  readonly responseType: typeof CommonReturnTypes_pb.StringMessage;
};

type RegistrationServiceOpenWebPage = {
  readonly methodName: string;
  readonly service: typeof RegistrationService;
  readonly requestStream: false;
  readonly responseStream: false;
  readonly requestType: typeof CommonReturnTypes_pb.StringMessage;
  readonly responseType: typeof CommonReturnTypes_pb.VoidMessage;
};

export class RegistrationService {
  static readonly serviceName: string;
  static readonly StopServices: RegistrationServiceStopServices;
  static readonly StartServices: RegistrationServiceStartServices;
  static readonly ReadServiceVersion: RegistrationServiceReadServiceVersion;
  static readonly OpenWebPage: RegistrationServiceOpenWebPage;
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

export class RegistrationServiceClient {
  readonly serviceHost: string;

  constructor(serviceHost: string, options?: grpc.RpcOptions);
  stopServices(
    requestMessage: CommonReturnTypes_pb.RegistrationData,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.BoolMessage|null) => void
  ): UnaryResponse;
  stopServices(
    requestMessage: CommonReturnTypes_pb.RegistrationData,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.BoolMessage|null) => void
  ): UnaryResponse;
  startServices(
    requestMessage: CommonReturnTypes_pb.VoidMessage,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.RegistrationData|null) => void
  ): UnaryResponse;
  startServices(
    requestMessage: CommonReturnTypes_pb.VoidMessage,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.RegistrationData|null) => void
  ): UnaryResponse;
  readServiceVersion(
    requestMessage: CommonReturnTypes_pb.VoidMessage,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  readServiceVersion(
    requestMessage: CommonReturnTypes_pb.VoidMessage,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.StringMessage|null) => void
  ): UnaryResponse;
  openWebPage(
    requestMessage: CommonReturnTypes_pb.StringMessage,
    metadata: grpc.Metadata,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
  openWebPage(
    requestMessage: CommonReturnTypes_pb.StringMessage,
    callback: (error: ServiceError|null, responseMessage: CommonReturnTypes_pb.VoidMessage|null) => void
  ): UnaryResponse;
}

