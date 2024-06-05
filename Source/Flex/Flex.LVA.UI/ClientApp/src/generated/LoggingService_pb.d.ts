// package: Flex.LVA.Communication.v1
// file: LoggingService.proto

import * as jspb from "google-protobuf";
import * as CommonReturnTypes_pb from "./CommonReturnTypes_pb";

export class LoggingRequest extends jspb.Message {
  getFunction(): string;
  setFunction(value: string): void;

  getDomain(): string;
  setDomain(value: string): void;

  getTracemessage(): string;
  setTracemessage(value: string): void;

  getThreadid(): number;
  setThreadid(value: number): void;

  getProcessid(): number;
  setProcessid(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): LoggingRequest.AsObject;
  static toObject(includeInstance: boolean, msg: LoggingRequest): LoggingRequest.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: LoggingRequest, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): LoggingRequest;
  static deserializeBinaryFromReader(message: LoggingRequest, reader: jspb.BinaryReader): LoggingRequest;
}

export namespace LoggingRequest {
  export type AsObject = {
    pb_function: string,
    domain: string,
    tracemessage: string,
    threadid: number,
    processid: number,
  }
}

