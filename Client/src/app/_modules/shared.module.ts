import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastrModule } from 'ngx-toastr';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxSpinnerModule } from 'ngx-spinner';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TabsModule.forRoot(),
    ToastrModule.forRoot({
      positionClass : 'toast-bottom-right'
    }),
    BsDropdownModule.forRoot(),
    NgxSpinnerModule.forRoot({ type: 'line-scale-party' })
  ],
  exports:[
    ToastrModule,
    BsDropdownModule,
    TabsModule,
    NgxSpinnerModule
  ]
})
export class SharedModule { }
