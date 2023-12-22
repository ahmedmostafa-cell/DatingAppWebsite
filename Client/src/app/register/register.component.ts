import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm :FormGroup = new FormGroup({});
  maxDate:Date = new Date();
  validationErrors : string[] | undefined;
  constructor(private accountservice : AccountService , private toastr : ToastrService , private fb:FormBuilder , private route :Router) { }

  @Output() cancelRegiter = new EventEmitter();

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() -18);
  }
  initializeForm()
  {
    this.registerForm = this.fb.group({
      gender : ["male"],
      username : ["" , Validators.required],
      knownAs : ["" , Validators.required],
      dateOfBirth : ["" , Validators.required],
      city : ["" , Validators.required],
      country : ["" , Validators.required],
      password : ["" ,[Validators.required , Validators.minLength(4) , Validators.maxLength(8)]],
      cofirmPassword : ["" ,  [Validators.required , this.matchValues('password')]],
    })
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: ()=> this.registerForm.controls['cofirmPassword'].updateValueAndValidity()
    })
  }
  matchValues(matchTo:string) :ValidatorFn
  {
    return (control:AbstractControl)=>{
      return control.value === control.parent?.get(matchTo)?.value ? null : {notmatching:true}
    }
  }
   register()
   {

    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value , dateOfBirth:dob};
    console.log("ahmedddd");
    console.log(values);
    this.accountservice.register(values).subscribe({
      next: () => {

        this.route.navigateByUrl('/members')

      },
      error: error => {

       this.validationErrors = error;
       console.log("ahmedddd");
        console.log(error.error);

      }
    })

   }

   cancell()
   {
    this.cancelRegiter.emit(false);
   }

   private getDateOnly(dob:string |undefined)
   {
    if(!dob) return;
    let thedob = new Date(dob);
    return new Date(thedob.setMinutes(thedob.getMinutes()-thedob.getTimezoneOffset())).toISOString().slice(0,10);
   }
}
