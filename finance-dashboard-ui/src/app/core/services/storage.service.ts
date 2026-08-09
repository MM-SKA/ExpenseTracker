import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable({
  providedIn: 'root'
})

export class StorageService {
  private readonly secretKey = 'finance-dashboard-secret-key';

  setEncrypted(key: string, value: unknown): void {
    if (typeof localStorage === 'undefined') return;
    try {
      const encrypted = CryptoJS.AES.encrypt(JSON.stringify(value), this.secretKey).toString();
      localStorage.setItem(key, encrypted);
    } catch (e) {
      console.error('Error encrypting storage item:', e);
    }
  }

  getEncrypted<T>(key: string): T | null {
    if (typeof localStorage === 'undefined') return null;
    const raw = localStorage.getItem(key);
    if (!raw) {
      return null;
    }
    try {
      const bytes = CryptoJS.AES.decrypt(raw, this.secretKey);
      const decrypted = bytes.toString(CryptoJS.enc.Utf8);
      if (decrypted) {
        return JSON.parse(decrypted) as T;
      }
    } catch {
      // Ignore decryption failure and try plain JSON fallback
    }
    try {
      return JSON.parse(raw) as T;
    } catch {
      return null;
    }
  }

  remove(key: string): void {
    if (typeof localStorage === 'undefined') return;
    localStorage.removeItem(key);
  }
}
