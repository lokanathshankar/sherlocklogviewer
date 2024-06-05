import { Injectable } from "@angular/core";
import { MessageService } from "primeng/api";
import { MessagingConstants } from "../app/FacsAndConsts";

@Injectable({
  providedIn: 'root'
})
export class MessageHelper {
  constructor(
    private myMessageService: MessageService) {
  }

  PostError(theMessage: string): void {
    this.myMessageService.add({
      severity: 'error',
      key: MessagingConstants.GlobalToast,
      detail: theMessage
    });
  }

  PostSuccess(theMessage: string): void {
    this.myMessageService.add({
      severity: 'success',
      key: MessagingConstants.GlobalToast,
      detail: theMessage
    });
  }
}
