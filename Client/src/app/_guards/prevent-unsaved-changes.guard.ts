import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  if(component.editForm?.dirty)
  {
    return confirm("Are You Sure You Want To Continue? Any Unsaved Changes Will Be Lost")
  }
    return true;
};
