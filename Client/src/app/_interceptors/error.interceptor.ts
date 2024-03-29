import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';



@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router : Router , private toastr : ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error:HttpErrorResponse)=> {
        switch(error.status) {
          case 400 :
            if(error.error.errors) {
              const modelStateErrorss =[];
              for(const key in error.error.errors)
              {
                if(error.error.errors[key])
                {
                  modelStateErrorss.push(error.error.errors[key])
                }



              }
              throw modelStateErrorss.flat();
            }
            else
            {
              this.toastr.error(error.error , error.status.toString());
            }
            break;
          case 401:
            this.toastr.error('Un Authorized' , error.status.toString());
            break;
          case 404 :
            this.router.navigateByUrl('/not-found');
            break;
          case 500 :
            const navigationextra :NavigationExtras = {state :{error : error.error}}
            this.router.navigateByUrl('/server-error' , navigationextra);
            break;
          default:
            this.toastr.error("something unexpected went wrong");
            console.log(error);
            break;

        }
        throw error;
      })
    )
  }
}
