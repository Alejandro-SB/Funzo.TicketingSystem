import type { Err } from 'src/types/utils';
import { type Result } from 'src/types/utils';
import { useApi } from './api';

export type CreateUserError =
  | UsernameAlreadyExistsError
  | UsernameNotValidError
  | DisplayNameNotValidError;

type UsernameAlreadyExistsError = Err<'UsernameAlreadyExists', object>;

type UsernameNotValidError = Err<
  'UsernameNotValid',
  {
    reason: string;
  }
>;

type DisplayNameNotValidError = Err<
  'DisplayNameNotValid',
  {
    reason: string;
  }
>;

export const useUsersApi = () => {
  const api = useApi();
  const createUser = (username: string, displayName: string) => {
    return api.post<Result<number, CreateUserError>>('/api/users', {
      username,
      displayName,
    });
  };

  return { createUser };
};
