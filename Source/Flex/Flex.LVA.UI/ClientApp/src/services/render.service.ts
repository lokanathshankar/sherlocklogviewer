
import { Injectable } from "@angular/core";
import { grpc } from "@improbable-eng/grpc-web";
import { Observable, Observer, Subject } from "rxjs";
import { GlobalProgressBarComponent } from "../app/global-progress-bar/global-progress-bar.component";
import { GlobalProgressBarEvents } from "../app/global-progress-bar/GloblaProgressBarEvents";
import { ILogHeader } from "../common/loggraph";
import { ISymanticLogs } from "../common/loggraph";
import { BoolMessage, IntMessage, RegistrationData, StringMessage, VoidMessage } from "../generated/CommonReturnTypes_pb";
import { LoggingRequest } from "../generated/LoggingService_pb";
import { LoggingServiceClient, ServiceError } from "../generated/LoggingService_pb_service";
import { RegistrationService, RegistrationServiceClient } from "../generated/RegistrationService_pb_service";
import { Tracer } from "../utils/tracer";
import { ServiceSettings } from "./constants";
import { Engine } from "./engine.service";
import { Logger } from "./logger.service";
class WebsocketAndTraceWrapper {
  Logger: Tracer;
  Socket: WebSocket;

  constructor(theSocket: WebSocket, theLogger: Tracer) {
    this.Socket = theSocket;
    this.Logger = theLogger;
  }
}

@Injectable({
  providedIn: 'root'
})
export class TableRenderer {
  private myDomain: string = "Renderer";
  private myRegistrationData = new RegistrationData();
  private myRegClient: RegistrationServiceClient;
  private myEndPointObserver: Subject<void> = new Subject();
  private myDataObserver?: Observer<ISymanticLogs>;
  private myHeaderObserver?: Observer<ILogHeader>;
  private myRenderObserver: Subject<void> = new Subject<void>();
  public rendererReady: Subject<void> = new Subject<void>();
  public Engine?: Engine;
  constructor(private myLogger: Logger, private myGlobalProgressBar: GlobalProgressBarEvents) {
    this.myRegClient = new RegistrationServiceClient(ServiceSettings.GRPC_REGISTERAR_ENDPOINT);
    this.myEndPointObserver.asObservable().subscribe(() => {
      this.BeginEndPointSetup();
      this.Engine = new Engine(myLogger, this.myRegistrationData);
      this.rendererReady.next();
    });
  }

  private BeginWebSocketConnect(theChannel: string): WebsocketAndTraceWrapper {
    const aTrace = new Tracer(this.myLogger, this.myDomain, `BeginWebSocketConnect.${this.myRegistrationData.getId()}.${theChannel}`);
    aTrace.info(`Beginning Websocket Connections With Base Address ${this.myRegistrationData.getRegistrationaddress()}`)
    const aBaseAddress = this.myRegistrationData.getRegistrationaddress();
    const aFullAddress = `${aBaseAddress}/${theChannel}`
    aTrace.info(`Opening Websocket With ${aFullAddress}`)
    const websocket = new WebSocket(aFullAddress);

    websocket.onopen = (event: Event) => {
      aTrace.info(`Websocket Opened @ ${aFullAddress}`)
    };

    websocket.onclose = function (event) {
      if (event.wasClean) {
        aTrace.info(`Connection closed cleanly, code=${event.code} @ ${aFullAddress}`);
      } else {
        aTrace.error(`Connection died @ ${aFullAddress}`);
      }
    };

    websocket.onerror = function (evt) {
      aTrace.error(`Connection Error @ ${aFullAddress}`);
    };

    aTrace.info(`Websocket Created @ ${aBaseAddress} ReadyState ${websocket.readyState}`)
    return new WebsocketAndTraceWrapper(websocket, aTrace);
  }

  HeaderChanges(): Observable<ILogHeader> {
    return new Observable(theObserver => {
      this.myHeaderObserver = theObserver;
    });
  }

  DataChanges(): Observable<ISymanticLogs> {
    return new Observable(theObserver => {
      this.myDataObserver = theObserver;
    });
  }

  RenderSubject(): Subject<void> {
    return this.myRenderObserver;
  }


  Start() {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "Start");
    this.myRegClient.startServices(new VoidMessage(), (error: ServiceError | null, responseMessage: RegistrationData | null) => {
      if (error != null) {
        aTrace.error(`Error startServices Client : ${error}`);
        return;
      }

      if (responseMessage == null) {
        aTrace.error('Null Response Message Is UnAcceptable');
        return;
      }

      if (responseMessage.getId() <= 0) {
        aTrace.error('Engine ID Cannot Be Zero');
        return;
      }

      this.myRegistrationData = responseMessage;
      aTrace.debug(`Registration Passed With ID : ${responseMessage.getId()} And Address ${responseMessage.getRegistrationaddress()}`);
      this.myEndPointObserver.next();
    });
  }



  private BeginEndPointSetup() {
    {
      const aPingId = "PingAlive";
      const aSocket = this.BeginWebSocketConnect(aPingId);
      aSocket.Socket.onmessage = (evt) => {
        aSocket.Logger.verbose(`${aPingId} Websocket Sending Reply for ${evt.data}`)
        aSocket.Socket.send(JSON.stringify(true));
      };
    }

    {
      const aNegotiateHeaderId = "NegotiateHeader";
      const aSocket = this.BeginWebSocketConnect(aNegotiateHeaderId);
      aSocket.Socket.onmessage = (evt) => {
        aSocket.Logger.info(`${aNegotiateHeaderId} Websocket Sending Reply for ${evt.data}`)
        aSocket.Socket.send(JSON.stringify(true));
        console.log(evt.data)
        this.myHeaderObserver?.next(JSON.parse(evt.data));
      };
    }

    {
      const aNegotiateDataId = "NegotiateData";
      const aSocket = this.BeginWebSocketConnect(aNegotiateDataId);
      aSocket.Socket.onmessage = (evt) => {
        const aSymLogs = JSON.parse(evt.data);
        aSocket.Socket.send(JSON.stringify(true));
        this.myDataObserver?.next(aSymLogs);
      };
    }

    {
      const aNegotiateRenderId = "RenderLogs";
      const aSocket = this.BeginWebSocketConnect(aNegotiateRenderId);
      aSocket.Socket.onmessage = (evt) => {
        this.myRenderObserver.next();
        aSocket.Logger.debug(`${aNegotiateRenderId} Websocket Sending Reply for ${evt.data}`)
        aSocket.Socket.send(JSON.stringify(true));
      };
    }


    {
      const aProgressId = "UpdateProgress";
      const aSocket = this.BeginWebSocketConnect(aProgressId);
      aSocket.Socket.onmessage = (evt) => {
        const aPercent = JSON.parse(evt.data);
        this.myGlobalProgressBar.updateProgressPercent.next(aPercent);
        aSocket.Logger.debug(`${aProgressId} Websocket Sending Reply for ${evt.data}`)
        aSocket.Socket.send(JSON.stringify(true));
      };
    }
  }
}
