
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { LogDefinition } from "../common/logdefinition";
import { UserWorkBoxData } from "../common/userworkboxdata";
import { ObjectUtils } from "../utils/objectutils";
import { Tracer } from "../utils/tracer";
import { Logger } from "./logger.service";

export enum UserBoxModeType {
  Local = "Local",
  //Server = "Sever",
  //Both = "Both"
}

export interface IUserWorkboxService {
  CurrentUserId: string;
  UserBoxLoaded: Subject<void>;
  UpdateUserBoxMode(theUserBoxMode: UserBoxModeType): boolean;
  LoadUserWorkBox(theUserId: string, theUserBoxMode: UserBoxModeType): boolean;
  AddToKnownPattern(theDefinition: LogDefinition): boolean;
  GetKnownPatterns(): Array<LogDefinition> | undefined;
  GetKnownPatternCount(): number;
}

@Injectable({
  providedIn: 'root'
})
export abstract class UserWorkboxService implements IUserWorkboxService {

  CurrentUserId: string = "";
  UserBoxLoaded: Subject<void> = new Subject<void>();
  private myUserBoxData: UserWorkBoxData | null = null;
  private myWorkingMode: UserBoxModeType = UserBoxModeType.Local;
  private myLogger: Logger = new Logger();
  private myDomain: string = "UserWorkboxService";
  LoadUserWorkBox(theUserId: string): boolean {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "LoadUserWorkBox");
    if (this.myWorkingMode == UserBoxModeType.Local) {
      if (!this.LoadUserDataFromLocalStorage(theUserId)) {
        if (!this.CreateWorkBoxForUserLocally(theUserId)) {
          aTrace.error("Error creating new workbok");
        }
        else {
          this.UserBoxLoaded.next();
        }
      }
    }

    this.CurrentUserId = theUserId;
    this.UserBoxLoaded.next();
    return true;
  }

  GetWorkBoxData(): UserWorkBoxData {
    return ObjectUtils.CloneObject(this.myUserBoxData);
  }

  ImportWorkBoxData(theDataToLoad: UserWorkBoxData): boolean {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "ImportWorkBoxData");
    this.myUserBoxData = theDataToLoad;
    if (!this.PutUserDataFromLocalStorage(this.CurrentUserId)) {
      aTrace.error("Error importing workbox data");
      return false;
    }
    else {
      this.UserBoxLoaded.next();
      return true;
    }
  }

  private CreateWorkBoxForUserLocally(theUserId: string): boolean {
    if (theUserId.length == 0) {
      return false;
    }

    this.CurrentUserId = theUserId;
    this.myUserBoxData = new UserWorkBoxData();
    return this.PutUserDataFromLocalStorage(theUserId);
  }


  UpdateUserBoxMode(theUserBoxMode: UserBoxModeType): boolean {
    throw new Error("Method not implemented.");
  }

  AddToKnownPattern(theDefinition: LogDefinition): boolean {
    if (this.myUserBoxData == null) { return false }
    for (var i = 0; i < this.myUserBoxData.SavedPatterns.length; i++) {
      if (LogDefinition.AreSame(theDefinition, this.myUserBoxData.SavedPatterns[i])) {
        return false;
      }
    }

    this.myUserBoxData.SavedPatterns.push(theDefinition);
    return this.PutUserDataFromLocalStorage(this.CurrentUserId);
  }

  GetKnownPatternCount(): number {
    if (this.myUserBoxData == null) {
      return 0;
    }

    return this.myUserBoxData.SavedPatterns.length;
  }

  GetKnownPatterns(): Array<LogDefinition> | undefined {
    return ObjectUtils.CloneObject(this.myUserBoxData?.SavedPatterns);
  }


  private PutUserDataFromLocalStorage(theUserId: string): boolean {
    if (theUserId.length == 0) {
      return false;
    }

    if (this.myUserBoxData == null) {
      return false;
    }

    const aClone = JSON.stringify(this.myUserBoxData)
    window.localStorage.setItem(theUserId, aClone)
    this.myUserBoxData = JSON.parse(aClone);
    return true;
  }

  private LoadUserDataFromLocalStorage(theUserId: string): boolean {
    if (theUserId.length == 0) {
      return false;
    }

    const aData = window.localStorage.getItem(theUserId);
    if (aData == null) {
      return false;
    }

    this.myUserBoxData = JSON.parse(aData);
    return true;
  }
}
