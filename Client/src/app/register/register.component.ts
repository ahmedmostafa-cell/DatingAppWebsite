import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private accountservice : AccountService , private toastr : ToastrService) { }

  @Output() cancelRegiter = new EventEmitter();
  model :any = {};
  ngOnInit(): void {
  }
   register()
   {
    this.accountservice.register(this.model).subscribe({
      next: () => {

        this.cancell();

      },
      error: error => {
        this.toastr.error(error.error);
        console.log(error);
      }
    })

   }

   cancell()
   {
    this.cancelRegiter.emit(false);
   }
}
