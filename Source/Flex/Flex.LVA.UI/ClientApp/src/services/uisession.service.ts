import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class UiSessionService {
  private mySessionResetSubject: Subject<void> = new Subject<void>();
  public sessionReset: Observable<void> = this.mySessionResetSubject.asObservable();
  public ResetSession() {
    this.mySessionResetSubject.next();
  }
}
