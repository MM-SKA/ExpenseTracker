import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})

export class StorageService {
  private readonly secretKey = 'finance-dashboard-secret-key';
  setEncrypted(key: string, value: unknown): void {
    const encrypted = CryptoJS.AES.encrypt(JSON.stringify(value), this.secretKey).toString();
    localStorage.setItem(key, encrypted);
  }
  getEncrypted<T>(key: string): T | null {
    const encrypted = localStorage.getItem(key);
    if (!encrypted) {
      return null;
    }
    const bytes = CryptoJS.AES.decrypt(encrypted, this.secretKey);
    const decrypted = bytes.toString(CryptoJS.enc.Utf8);
    return JSON.parse(decrypted) as T;
  }
  remove(key: string): void {
    localStorage.removeItem(key);
  }
}
