import { Inject, Injectable } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private themeDarkSubject = new Subject<boolean>();
  public themeDark$ = this.themeDarkSubject.asObservable();
  constructor(@Inject(DOCUMENT) private document: Document) { }

  switchTheme(theDark: boolean) {
    let themeLink = this.document.getElementById('app-theme') as HTMLLinkElement;
    {
      if (!themeLink) {
        return;
      }

      if (theDark) {
        themeLink.href = "prime-theme/main_theme_dark.css";
      }
      else {
        themeLink.href = "prime-theme/main_theme_light.css";
      }
      this.themeDarkSubject.next(theDark);

    }

    {
      let tableLink = this.document.getElementById('table-theme') as HTMLLinkElement;
      if (!tableLink) {
        return;
      }

      if (theDark) {
        tableLink.href = "tabulator-theme/tabulator_midnight.min.css";
      }
      else {
        tableLink.href = "tabulator-theme/tabulator_simple.min.css";
      }
    }
  }
}
