import { Component } from '@angular/core';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { UserWorkBoxData } from '../../common/userworkboxdata';
import { MessageHelper } from '../../services/messagehelper';
import { UserWorkboxService } from '../../services/userworkbox.service';

@Component({
  selector: 'app-session-login',
  templateUrl: './session-login.component.html',
  styleUrls: ['./session-login.component.css']
})
export class SessionLoginComponent {
  public enteredUserId: string = "GuestUser";
  constructor(
    public ref: DynamicDialogRef,
    private myUserWorkBoxSer: UserWorkboxService,
    private myDialogService: DialogService,
    private myDialogRef: DynamicDialogRef) {
  }

  login(): void {
    this.myUserWorkBoxSer.LoadUserWorkBox(this.enteredUserId);
    this.ref.close();
  }
}
