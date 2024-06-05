import { Component } from '@angular/core';
import { GlobalProgressBarEvents } from './GloblaProgressBarEvents';

@Component({
  selector: 'app-global-progress-bar',
  templateUrl: './global-progress-bar.component.html',
  styleUrls: ['./global-progress-bar.component.css']
})
export class GlobalProgressBarComponent {
  public visible: boolean = false;
  public activeMessage: string = "No message set";
  public progressValue: number = 0;
  public progressMode: string = "indeterminate";
  constructor(private myGlobalProgressEvents: GlobalProgressBarEvents) {
    this.myGlobalProgressEvents.showProgress.subscribe((theX: string) => {
      this.activeMessage = theX;
      this.visible = true;
    });

    this.myGlobalProgressEvents.hideProgress.subscribe(() => {
      this.visible = false;
    });

    this.myGlobalProgressEvents.updateProgressMessage.subscribe((theX) => {
      this.activeMessage = theX;
    });

    this.myGlobalProgressEvents.updateProgressPercent.subscribe((theX: number) => {
      if (theX == -1) {
        this.progressMode = "indeterminate"
      }

      this.progressValue = theX;
    });
  }
}
