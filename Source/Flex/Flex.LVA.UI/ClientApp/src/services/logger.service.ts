import { Injectable } from "@angular/core";
import { VoidMessage } from "../generated/CommonReturnTypes_pb";
import { LoggingRequest } from "../generated/LoggingService_pb";
import { LoggingServiceClient, ServiceError } from "../generated/LoggingService_pb_service";
import { ServiceSettings } from "./constants";

@Injectable({
  providedIn: 'root'
})
export class Logger {
  private client: LoggingServiceClient;
  constructor() {
    this.client = new LoggingServiceClient(ServiceSettings.GRPC_LOGGER_ENDPOINT);
  }

  info(logMessageRequest: LoggingRequest) {
    console.info(`${logMessageRequest.getDomain()}.${logMessageRequest.getFunction()} - ${logMessageRequest.getTracemessage()}`);
    this.client.info(logMessageRequest, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error) {
        console.error(error);
      }
    });
  }

  error(logMessageRequest: LoggingRequest) {
    console.error(`${logMessageRequest.getDomain()}.${logMessageRequest.getFunction()} - ${logMessageRequest.getTracemessage()}`);
    this.client.error(logMessageRequest, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error) {
        console.error(error);
      }
    });
  }

  debug(logMessageRequest: LoggingRequest) {
    this.client.debug(logMessageRequest, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error) {
        console.error(error);
      }
    });
  }

  verbose(logMessageRequest: LoggingRequest) {
    this.client.verbose(logMessageRequest, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error) {
        console.error(error);
      }
    });
  }

  warn(logMessageRequest: LoggingRequest) {
    this.client.warn(logMessageRequest, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      if (error) {
        console.error(error);
      }
    });
  }
}
