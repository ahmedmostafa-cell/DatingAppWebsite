import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private accountservice : AccountService) { }

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
        console.log(error);
      }
    })

   }

   cancell()
   {
    this.cancelRegiter.emit(false);
   }
}
