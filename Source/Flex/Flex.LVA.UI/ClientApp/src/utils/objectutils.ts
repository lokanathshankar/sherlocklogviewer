export class ObjectUtils {
  static CloneObject(theObject: any): any {
    const aClone = JSON.stringify(theObject)
    return JSON.parse(aClone);
  }

  static RemoveArrayElement<Type>(theArray: Type[], theObject: any): Type[] {
    const aIndex = theArray.indexOf(theObject);
    if (aIndex != -1) {
      theArray.splice(aIndex, 1);
    }

    return theArray;
  }
}
