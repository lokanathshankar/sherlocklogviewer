// source: LoggingService.proto
/**
 * @fileoverview
 * @enhanceable
 * @suppress {missingRequire} reports error on implicit type usages.
 * @suppress {messageConventions} JS Compiler reports an error if a variable or
 *     field starts with 'MSG_' and isn't a translatable message.
 * @public
 */
// GENERATED CODE -- DO NOT EDIT!
/* eslint-disable */
// @ts-nocheck

var jspb = require('google-protobuf');
var goog = jspb;
var global = (function() { return this || window || global || self || Function('return this')(); }).call(null);

var CommonReturnTypes_pb = require('./CommonReturnTypes_pb.js');
goog.object.extend(proto, CommonReturnTypes_pb);
goog.exportSymbol('proto.Flex.LVA.Communication.v1.LoggingRequest', null, global);
/**
 * Generated by JsPbCodeGenerator.
 * @param {Array=} opt_data Optional initial data array, typically from a
 * server response, or constructed directly in Javascript. The array is used
 * in place and becomes part of the constructed object. It is not cloned.
 * If no data is provided, the constructed object will be empty, but still
 * valid.
 * @extends {jspb.Message}
 * @constructor
 */
proto.Flex.LVA.Communication.v1.LoggingRequest = function(opt_data) {
  jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};
goog.inherits(proto.Flex.LVA.Communication.v1.LoggingRequest, jspb.Message);
if (goog.DEBUG && !COMPILED) {
  /**
   * @public
   * @override
   */
  proto.Flex.LVA.Communication.v1.LoggingRequest.displayName = 'proto.Flex.LVA.Communication.v1.LoggingRequest';
}



if (jspb.Message.GENERATE_TO_OBJECT) {
/**
 * Creates an object representation of this proto.
 * Field names that are reserved in JavaScript and will be renamed to pb_name.
 * Optional fields that are not set will be set to undefined.
 * To access a reserved field use, foo.pb_<name>, eg, foo.pb_default.
 * For the list of reserved names please see:
 *     net/proto2/compiler/js/internal/generator.cc#kKeyword.
 * @param {boolean=} opt_includeInstance Deprecated. whether to include the
 *     JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @return {!Object}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.toObject = function(opt_includeInstance) {
  return proto.Flex.LVA.Communication.v1.LoggingRequest.toObject(opt_includeInstance, this);
};


/**
 * Static version of the {@see toObject} method.
 * @param {boolean|undefined} includeInstance Deprecated. Whether to include
 *     the JSPB instance for transitional soy proto support:
 *     http://goto/soy-param-migration
 * @param {!proto.Flex.LVA.Communication.v1.LoggingRequest} msg The msg instance to transform.
 * @return {!Object}
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.toObject = function(includeInstance, msg) {
  var f, obj = {
    pb_function: jspb.Message.getFieldWithDefault(msg, 1, ""),
    domain: jspb.Message.getFieldWithDefault(msg, 2, ""),
    tracemessage: jspb.Message.getFieldWithDefault(msg, 3, ""),
    threadid: jspb.Message.getFieldWithDefault(msg, 4, 0),
    processid: jspb.Message.getFieldWithDefault(msg, 5, 0)
  };

  if (includeInstance) {
    obj.$jspbMessageInstance = msg;
  }
  return obj;
};
}


/**
 * Deserializes binary data (in protobuf wire format).
 * @param {jspb.ByteSource} bytes The bytes to deserialize.
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.deserializeBinary = function(bytes) {
  var reader = new jspb.BinaryReader(bytes);
  var msg = new proto.Flex.LVA.Communication.v1.LoggingRequest;
  return proto.Flex.LVA.Communication.v1.LoggingRequest.deserializeBinaryFromReader(msg, reader);
};


/**
 * Deserializes binary data (in protobuf wire format) from the
 * given reader into the given message object.
 * @param {!proto.Flex.LVA.Communication.v1.LoggingRequest} msg The message object to deserialize into.
 * @param {!jspb.BinaryReader} reader The BinaryReader to use.
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.deserializeBinaryFromReader = function(msg, reader) {
  while (reader.nextField()) {
    if (reader.isEndGroup()) {
      break;
    }
    var field = reader.getFieldNumber();
    switch (field) {
    case 1:
      var value = /** @type {string} */ (reader.readString());
      msg.setFunction(value);
      break;
    case 2:
      var value = /** @type {string} */ (reader.readString());
      msg.setDomain(value);
      break;
    case 3:
      var value = /** @type {string} */ (reader.readString());
      msg.setTracemessage(value);
      break;
    case 4:
      var value = /** @type {number} */ (reader.readUint32());
      msg.setThreadid(value);
      break;
    case 5:
      var value = /** @type {number} */ (reader.readUint32());
      msg.setProcessid(value);
      break;
    default:
      reader.skipField();
      break;
    }
  }
  return msg;
};


/**
 * Serializes the message to binary data (in protobuf wire format).
 * @return {!Uint8Array}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.serializeBinary = function() {
  var writer = new jspb.BinaryWriter();
  proto.Flex.LVA.Communication.v1.LoggingRequest.serializeBinaryToWriter(this, writer);
  return writer.getResultBuffer();
};


/**
 * Serializes the given message to binary data (in protobuf wire
 * format), writing to the given BinaryWriter.
 * @param {!proto.Flex.LVA.Communication.v1.LoggingRequest} message
 * @param {!jspb.BinaryWriter} writer
 * @suppress {unusedLocalVariables} f is only used for nested messages
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.serializeBinaryToWriter = function(message, writer) {
  var f = undefined;
  f = message.getFunction();
  if (f.length > 0) {
    writer.writeString(
      1,
      f
    );
  }
  f = message.getDomain();
  if (f.length > 0) {
    writer.writeString(
      2,
      f
    );
  }
  f = message.getTracemessage();
  if (f.length > 0) {
    writer.writeString(
      3,
      f
    );
  }
  f = message.getThreadid();
  if (f !== 0) {
    writer.writeUint32(
      4,
      f
    );
  }
  f = message.getProcessid();
  if (f !== 0) {
    writer.writeUint32(
      5,
      f
    );
  }
};


/**
 * optional string Function = 1;
 * @return {string}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.getFunction = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 1, ""));
};


/**
 * @param {string} value
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest} returns this
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.setFunction = function(value) {
  return jspb.Message.setProto3StringField(this, 1, value);
};


/**
 * optional string Domain = 2;
 * @return {string}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.getDomain = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 2, ""));
};


/**
 * @param {string} value
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest} returns this
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.setDomain = function(value) {
  return jspb.Message.setProto3StringField(this, 2, value);
};


/**
 * optional string TraceMessage = 3;
 * @return {string}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.getTracemessage = function() {
  return /** @type {string} */ (jspb.Message.getFieldWithDefault(this, 3, ""));
};


/**
 * @param {string} value
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest} returns this
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.setTracemessage = function(value) {
  return jspb.Message.setProto3StringField(this, 3, value);
};


/**
 * optional uint32 ThreadId = 4;
 * @return {number}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.getThreadid = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 4, 0));
};


/**
 * @param {number} value
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest} returns this
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.setThreadid = function(value) {
  return jspb.Message.setProto3IntField(this, 4, value);
};


/**
 * optional uint32 ProcessId = 5;
 * @return {number}
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.getProcessid = function() {
  return /** @type {number} */ (jspb.Message.getFieldWithDefault(this, 5, 0));
};


/**
 * @param {number} value
 * @return {!proto.Flex.LVA.Communication.v1.LoggingRequest} returns this
 */
proto.Flex.LVA.Communication.v1.LoggingRequest.prototype.setProcessid = function(value) {
  return jspb.Message.setProto3IntField(this, 5, value);
};


goog.object.extend(exports, proto.Flex.LVA.Communication.v1);
