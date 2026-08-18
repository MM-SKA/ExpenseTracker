import {
  TestBed
} from '@angular/core/testing';

import {
  afterEach,
  beforeEach,
  describe,
  expect,
  it,
  vi
} from 'vitest';

import {
  StorageService
} from './storage.service';

describe('StorageService', () => {

  let service: StorageService;

  beforeEach(() => {

    TestBed.configureTestingModule({});

    service =
      TestBed.inject(StorageService);

    localStorage.clear();

  });

  afterEach(() => {

    vi.restoreAllMocks();

    localStorage.clear();

  });

  it('should be created', () => {

    expect(service)
      .toBeTruthy();

  });

  it('should store encrypted value in localStorage', () => {

    const key =
      'test-key';

    const value = {
      id: '1',
      name: 'Food'
    };

    service.setEncrypted(
      key,
      value
    );

    const rawValue =
      localStorage.getItem(key);

    expect(rawValue)
      .toBeTruthy();

    expect(rawValue)
      .not
      .toBe(
        JSON.stringify(value)
      );

  });

  it('should decrypt and return stored encrypted value', () => {

    const key =
      'user';

    const value = {
      id: 'user-123',
      fullName: 'Test User',
      email: 'test@test.com'
    };

    service.setEncrypted(
      key,
      value
    );

    const result =
      service.getEncrypted<typeof value>(
        key
      );

    expect(result)
      .toEqual(value);

  });

  it('should return null when key does not exist', () => {

    const result =
      service.getEncrypted(
        'missing-key'
      );

    expect(result)
      .toBeNull();

  });

  it('should return plain JSON value as fallback when value is not encrypted', () => {

    const key =
      'plain-json-key';

    const value = {
      id: 'category-1',
      name: 'Food',
      isSystemCategory: true
    };

    localStorage.setItem(
      key,
      JSON.stringify(value)
    );

    const result =
      service.getEncrypted<typeof value>(
        key
      );

    expect(result)
      .toEqual(value);

  });

  it('should return null when stored value is neither encrypted nor valid JSON', () => {

    const key =
      'invalid-key';

    localStorage.setItem(
      key,
      'not-valid-json'
    );

    const result =
      service.getEncrypted(
        key
      );

    expect(result)
      .toBeNull();

  });

  it('should remove item from localStorage', () => {

    const key =
      'remove-key';

    localStorage.setItem(
      key,
      'some-value'
    );

    service.remove(key);

    expect(
      localStorage.getItem(key)
    ).toBeNull();

  });

  it('should log error when encryption fails', () => {

    const consoleErrorSpy =
      vi.spyOn(
        console,
        'error'
      ).mockImplementation(() => undefined);

    const circularValue: Record<string, unknown> = {};

    circularValue['self'] =
      circularValue;

    service.setEncrypted(
      'broken-key',
      circularValue
    );

    expect(consoleErrorSpy)
      .toHaveBeenCalled();

    expect(
      localStorage.getItem(
        'broken-key'
      )
    ).toBeNull();

  });

});
