import { Injectable } from "@angular/core";
import { LoggingRequest } from "../generated/LoggingService_pb";
import { Logger } from "../services/logger.service";
export function timer() {
  let timeStart = new Date().getTime();
  return {
    /** <integer>s e.g 2s etc. */
    get seconds() {
      const seconds = Math.ceil((new Date().getTime() - timeStart) / 1000) + 's';
      return seconds;
    },
    /** Milliseconds e.g. 2000ms etc. */
    get ms() {
      const ms = (new Date().getTime() - timeStart) + 'ms';
      return ms;
    }
  }
}

export class Tracer {
  constructor(private logger: Logger, private domain: string, private funcname: string)
  {
  }

  public debug(msg: string) {
    const aRequest = new LoggingRequest();
    aRequest.setTracemessage(msg);
    aRequest.setFunction(this.funcname);
    aRequest.setDomain(this.domain);
    this.logger.debug(aRequest);
  }

  public info(msg: any) {
    const aRequest = new LoggingRequest();
    aRequest.setTracemessage(JSON.stringify(msg));
    aRequest.setFunction(this.funcname);
    aRequest.setDomain(this.domain);
    this.logger.info(aRequest);
  }

  public error(msg: string) {
    const aRequest = new LoggingRequest();
    aRequest.setTracemessage(msg);
    aRequest.setFunction(this.funcname);
    aRequest.setDomain(this.domain);
    this.logger.error(aRequest);
  }

  public verbose(msg: string) {
    const aRequest = new LoggingRequest();
    aRequest.setTracemessage(msg);
    aRequest.setFunction(this.funcname);
    aRequest.setDomain(this.domain);
    this.logger.verbose(aRequest);
  }

  public warn(msg: string) {
    const aRequest = new LoggingRequest();
    aRequest.setTracemessage(msg);
    aRequest.setFunction(this.funcname);
    aRequest.setDomain(this.domain);
    this.logger.warn(aRequest);
  }
}
