import type { Union, Option } from 'src/types/utils';
import { type Result } from 'src/types/utils';
import { useApi } from './api';

export type CreateUserError =
  | UsernameAlreadyExistsError
  | UsernameNotValidError
  | DisplayNameNotValidError;

type UsernameAlreadyExistsError = Union<'UsernameAlreadyExists', object>;

type UsernameNotValidError = Union<
  'UsernameNotValid',
  {
    reason: string;
  }
>;

type DisplayNameNotValidError = Union<
  'DisplayNameNotValid',
  {
    reason: string;
  }
>;

export type GetUserResponse = {
  id: number;
  username: string;
  displayName: string;
};

export type GetAllUsersResponse = {
  users: GetAllUsersUser[];
};

export type GetAllUsersUser = {
  id: number;
  username: string;
  displayName: string;
  userComments: number;
};

export const useUsersApi = () => {
  const api = useApi();
  const createUser = (username: string, displayName: string) => {
    return api.post<Result<number, CreateUserError>>('/api/users', {
      username,
      displayName,
    });
  };

  const getUser = (id: number) => {
    return api.get<Option<GetUserResponse>>(`/api/users/${id}`);
  };

  const getAllUsers = () => {
    return api.get<GetAllUsersResponse>('/api/users');
  };

  return { createUser, getUser, getAllUsers };
};
