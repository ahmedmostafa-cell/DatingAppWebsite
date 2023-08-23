import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model :any = {}

  constructor(public accountservice:AccountService , private router : Router
     , private toastr : ToastrService) { }

  ngOnInit(): void {

  }


  logIn() {
    this.accountservice.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.toastr.success("success log in");

      },
      error: error => {
       
      }

    })
  }

  loggedOut()
  {
    this.accountservice.logout();
    this.toastr.success("success log out");
    this.router.navigateByUrl('/');


  }

}
