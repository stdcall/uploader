import { Component } from '@angular/core';
import { UploadService } from '../_services/upload.service';
import { AlertService } from '../_services/alert.service';

import { Upload } from '../_models/upload';
import { User } from '../_models/user';
@Component({
    selector: 'uploads',
    templateUrl: './uploads.component.html'
  })
  
  export class UploadsComponent {
    public uploads: Upload[];
    public progress: number;
    public currentUser: User;
    constructor(private uploadService: UploadService, 
                private alertService: AlertService) { 
                    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
                }
    
    ngOnInit() {
        this.loadUploads();
    }

    loadUploads() {
        this.uploadService.loadUploads().subscribe(
            data => {
                this.uploads = data;
            },
            error => {
                console.log(error);
                this.alertService.error(error.message);
            }
        );
    }

    upload(files) {
        if (files.length === 0) return;
        
        this.uploadService.upload(files).subscribe(
            _ => {
                this.alertService.success("Загружено.");
                this.loadUploads();
            },
            error => {
                this.alertService.error(error.error.message);
                console.log(error);
            }
        );
    }

    delete(id) {
        this.uploadService.delete(id).subscribe(
            _=> {
                this.alertService.success("Файл удален.");
                this.loadUploads();
            },
            error => {
                this.alertService.error(error.error.message);
                console.log(error);
            }
        );
    }
  }