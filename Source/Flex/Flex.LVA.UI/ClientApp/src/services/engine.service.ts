
import { A11yModule } from "@angular/cdk/a11y";
import { Injectable } from "@angular/core";
import { grpc } from "@improbable-eng/grpc-web";
import { Observable, Observer, Subject, Subscriber } from "rxjs";
import { ParserEventsAndUpdates } from "../app/parser-setup-view/ParserEvents";
import { LogDefinition } from "../common/logdefinition";
import { BoolMessage, IntMessage, RegistrationData, StringMessage, VoidMessage } from "../generated/CommonReturnTypes_pb";
import { GetRawLogRequest, GetRawLogsRequest, LogChunkRequest, PrepareResourceRequest } from "../generated/EngineService_pb";
import { EngineServiceClient } from "../generated/EngineService_pb_service";
import { LoggingRequest } from "../generated/LoggingService_pb";
import { LoggingServiceClient, ServiceError } from "../generated/LoggingService_pb_service";
import { RegistrationService, RegistrationServiceClient } from "../generated/RegistrationService_pb_service";
import { Tracer } from "../utils/tracer";
import { ServiceSettings } from "./constants";
import { Logger } from "./logger.service";
export class Engine {
  private myDomain: string = "Engine";
  private myEngineClient: EngineServiceClient;
  private myRawLogObserver?: Observer<string>;
  private myLogDefintion: string = "";
  public prepareLogFailed: Subject<void> = new Subject();
  public appendLogFailed: Subject<void> = new Subject();
  constructor(private myLogger: Logger, private myRegistrationData: RegistrationData) {
    this.myEngineClient = new EngineServiceClient(ServiceSettings.GRPC_ENGINE_ENDPOINT);
  }

  copyRawToClipBoard(theId: number): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, `getRawLog.${this.myRegistrationData.getId()}`);
    const aRawRequest = new GetRawLogRequest();
    aRawRequest.setRegistrationdata(this.myRegistrationData);
    aRawRequest.setLogid(theId);
    this.myEngineClient.getRawLog(aRawRequest, (error: ServiceError | null, responseMessage: StringMessage | null) => {
      if (error != null) {
        aTrace.error(`Error getRawLog Clipboard Client : ${error}`);
        return;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return;
      }

      navigator.clipboard.writeText(responseMessage.getValue());
    });
  }

  getRawLog(theId: number): Observable<string> {
    const aTrace = new Tracer(this.myLogger, this.myDomain, `getRawLog.${this.myRegistrationData.getId()}`);
    const aRawRequest = new GetRawLogRequest();
    aRawRequest.setRegistrationdata(this.myRegistrationData);
    aRawRequest.setLogid(theId);
    this.myEngineClient.getRawLog(aRawRequest, (error: ServiceError | null, responseMessage: StringMessage | null) => {
      if (error != null) {
        aTrace.error(`Error getRawLog Client : ${error}`);
        return;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return;
      }

      this.myRawLogObserver?.next(responseMessage.getValue());
    });

    return new Observable(theObserver => {
      this.myRawLogObserver = theObserver;
    });
  }

  getRawLogs(theIds: number[]): Observable<string> {
    const aTrace = new Tracer(this.myLogger, this.myDomain, `getRawLog.${this.myRegistrationData.getId()}`);
    const aRawRequest = new GetRawLogsRequest();
    Observable
    aRawRequest.setRegistrationdata(this.myRegistrationData);
    aRawRequest.setLogidsList(theIds);
    let aLogsObserver: Subscriber<string>;
    this.myEngineClient.getRawLogs(aRawRequest, (error: ServiceError | null, responseMessage: StringMessage | null) => {
      if (error != null) {
        aTrace.error(`Error getRawLog Client : ${error}`);
        return;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return;
      }

      aLogsObserver.next(responseMessage.getValue());
    });

    return new Observable(theObserver => {
      aLogsObserver = theObserver;
    });
  }

  SetLogDefinition(theDefinition: LogDefinition) {
    this.myLogDefintion = JSON.stringify(theDefinition);
  }

  PrepareLogs(theFile: File): void {
    const aReq = new PrepareResourceRequest();
    aReq.setRegistrationdata(this.myRegistrationData);
    aReq.setLogdefinition(this.myLogDefintion);

    const aFileReader = new FileReader();
    const aSyncSubject = new Subject<void>();
    aFileReader.onloadend = (e: ProgressEvent) => {
      if (aFileReader.result == null) {
        return;
      }

      const aChunk = new LogChunkRequest();
      aChunk.setRegistrationdata(this.myRegistrationData);
      //@ts-ignore
      const aBytes = new Uint8Array(aFileReader.result);
      let aSliceStart = 0;
      let aSliceEnd = 0;

      const aPrepareSubject = new Subject<void>();
      aSyncSubject.subscribe(() => {
        aSliceEnd = aSliceStart + 4 * 1024 * 1024;
        aChunk.setLogchunk(aBytes.subarray(aSliceStart, aSliceEnd));
        aSliceStart = aSliceEnd;
        this.myEngineClient.appendLogChunk(aChunk, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
          const aTrace = new Tracer(this.myLogger, this.myDomain, `appendLogChunk.${this.myRegistrationData.getId()}`);
          if (error != null) {
            aTrace.error(`AppendLogChunk Failed : ${error}`);
            this.appendLogFailed.next();
            return;
          }

          if (responseMessage == null) {
            aTrace.error('Null Response For AppendLogChunk Is UnAcceptable');
            this.appendLogFailed.next();
            return;
          }

          if (aSliceEnd > aBytes.buffer.byteLength) {
            aPrepareSubject.next();
          }
          else {
            aSyncSubject.next();
          }
        });
      });

      aPrepareSubject.subscribe(() => {
        this.myEngineClient.prepareLogs(this.myRegistrationData, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
          const aTrace = new Tracer(this.myLogger, this.myDomain, `prepareLogs.${this.myRegistrationData.getId()}`);
          if (error != null) {
            aTrace.error(`Server error : ${error}`);
            this.prepareLogFailed.next();
            return;
          }

          if (responseMessage == null) {
            aTrace.error('Null Response Message Is UnAcceptable');
            this.prepareLogFailed.next();
            return;
          }
        });
      });

      aSyncSubject.next();
    }

    this.myEngineClient.prepareResources(aReq, (error: ServiceError | null, responseMessage: VoidMessage | null) => {
      const aTrace = new Tracer(this.myLogger, this.myDomain, `prepareResources.${this.myRegistrationData.getId()}`);
      if (error != null) {
        aTrace.error(`Server error : ${error}`);
        this.prepareLogFailed.next();
        return;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        this.prepareLogFailed.next();
        return;
      }

      aFileReader.readAsArrayBuffer(theFile);
    });
  }
}
