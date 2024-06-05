import { Subject } from "rxjs";

export class FileUtils {
  public static pickFile(theExtensions: string = "*.*", onFilePicked: (file: File) => void): Subject<void> {
    const aSubject = new Subject<void>();
    const inputElemenet = document.createElement('input');
    inputElemenet.style.display = 'none';
    inputElemenet.type = 'file';
    inputElemenet.accept = theExtensions;

    inputElemenet.addEventListener('change', () => {
      if (inputElemenet.files) {
        onFilePicked(inputElemenet.files[0]);
      }
    });

    const teardown = () => {
      document.body.removeEventListener('focus', teardown, true);
      setTimeout(() => {
        document.body.removeChild(inputElemenet);
      }, 1000);
    }
    document.body.addEventListener('focus', teardown, true);

    document.body.appendChild(inputElemenet);
    inputElemenet.click();
    return aSubject;
  }


  public static download(filename: string, theObject: any): void {
    const a: HTMLAnchorElement = document.createElement('a');
    a.style.display = 'none';
    document.body.appendChild(a);

    const aBlob = new Blob([JSON.stringify(theObject)], {
      type: 'text/plain'
    });
    const url: string = window.URL.createObjectURL(aBlob);

    a.href = url;
    a.download = filename;

    a.click();

    window.URL.revokeObjectURL(url);
    a.parentElement?.removeChild(a);
  };
}
