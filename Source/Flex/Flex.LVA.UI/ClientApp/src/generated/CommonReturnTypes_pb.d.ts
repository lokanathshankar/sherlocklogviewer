// package: Flex.LVA.Communication.v1
// file: CommonReturnTypes.proto

import * as jspb from "google-protobuf";

export class VoidMessage extends jspb.Message {
  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): VoidMessage.AsObject;
  static toObject(includeInstance: boolean, msg: VoidMessage): VoidMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: VoidMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): VoidMessage;
  static deserializeBinaryFromReader(message: VoidMessage, reader: jspb.BinaryReader): VoidMessage;
}

export namespace VoidMessage {
  export type AsObject = {
  }
}

export class BoolMessage extends jspb.Message {
  getValue(): boolean;
  setValue(value: boolean): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): BoolMessage.AsObject;
  static toObject(includeInstance: boolean, msg: BoolMessage): BoolMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: BoolMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): BoolMessage;
  static deserializeBinaryFromReader(message: BoolMessage, reader: jspb.BinaryReader): BoolMessage;
}

export namespace BoolMessage {
  export type AsObject = {
    value: boolean,
  }
}

export class StringMessage extends jspb.Message {
  getValue(): string;
  setValue(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): StringMessage.AsObject;
  static toObject(includeInstance: boolean, msg: StringMessage): StringMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: StringMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): StringMessage;
  static deserializeBinaryFromReader(message: StringMessage, reader: jspb.BinaryReader): StringMessage;
}

export namespace StringMessage {
  export type AsObject = {
    value: string,
  }
}

export class IntMessage extends jspb.Message {
  getValue(): number;
  setValue(value: number): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): IntMessage.AsObject;
  static toObject(includeInstance: boolean, msg: IntMessage): IntMessage.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: IntMessage, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): IntMessage;
  static deserializeBinaryFromReader(message: IntMessage, reader: jspb.BinaryReader): IntMessage;
}

export namespace IntMessage {
  export type AsObject = {
    value: number,
  }
}

export class RegistrationData extends jspb.Message {
  getId(): number;
  setId(value: number): void;

  getRegistrationaddress(): string;
  setRegistrationaddress(value: string): void;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): RegistrationData.AsObject;
  static toObject(includeInstance: boolean, msg: RegistrationData): RegistrationData.AsObject;
  static extensions: {[key: number]: jspb.ExtensionFieldInfo<jspb.Message>};
  static extensionsBinary: {[key: number]: jspb.ExtensionFieldBinaryInfo<jspb.Message>};
  static serializeBinaryToWriter(message: RegistrationData, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): RegistrationData;
  static deserializeBinaryFromReader(message: RegistrationData, reader: jspb.BinaryReader): RegistrationData;
}

export namespace RegistrationData {
  export type AsObject = {
    id: number,
    registrationaddress: string,
  }
}

