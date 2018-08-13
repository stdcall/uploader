import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Upload } from '../_models/upload';

@Injectable()
export class UploadService {
    constructor(private http: HttpClient) { }

    loadUploads() {
        return this.http.get<Upload[]>('/api/uploads');
    }

    upload(files) {
        const formData = new FormData();
        
        for (let file of files) {
            formData.append("files", file, file.name);
        }
        return this.http.post('/api/uploads/', formData, {reportProgress: true});
    }

    delete(id: number) {
        return this.http.delete('/api/uploads/' + id);
    }
}