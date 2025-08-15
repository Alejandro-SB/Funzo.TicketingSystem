import { useApi } from './api';
import type { Result, Union } from '../types/utils';
import { type Option } from '../types/utils';
import { useAuth } from 'src/composables/useAuth';
export type GetAllTicketsResponse = {
  tickets: GetAllTicketsTicket[];
};
export type GetAllTicketsTicket = {
  id: number;
  subject: string;
  totalComments: number;
};

export type GetTicketResponse = {
  id: number;
  subject: string;
  isEscalated: boolean;
  resolutionDate: string;
  comments: {
    userId: number;
    displayName: string;
    text: string;
  }[];
};

export type TicketNotFoundError = Union<'TicketNotFound', object>;
export type UserNotFoundError = Union<'UserNotFound', object>;
export type InvalidCommentTextError = Union<'InvalidCommentText', object>;
export type TicketAlreadySolvedError = Union<'TicketAlreadySolved', { resolutionDate: string }>;
export type TicketAlreadyEscalatedError = Union<'TicketAlreadyEscalated', object>;
export type InvalidTicketBodyError = Union<'InvalidTicketBody', { reason: string }>;
export type InvalidTicketSubjectError = Union<'InvalidTicketSubject', { reason: string }>;

export type AddCommentToTicketError =
  | TicketNotFoundError
  | UserNotFoundError
  | InvalidCommentTextError
  | TicketAlreadySolvedError;

export type EscalateTicketError =
  | TicketAlreadyEscalatedError
  | TicketAlreadySolvedError
  | TicketNotFoundError;

export type SolveTicketError = TicketNotFoundError | TicketAlreadySolvedError;

export type CreateTicketError =
  | UserNotFoundError
  | InvalidTicketBodyError
  | InvalidTicketSubjectError;

export const useTicketsApi = () => {
  const api = useApi();
  const { user } = useAuth();

  const getAllTickets = () => {
    return api.get<GetAllTicketsResponse>('/api/tickets');
  };

  const getTicket = (id: number) => {
    return api.get<Option<GetTicketResponse>>(`/api/tickets/${id}`);
  };

  const create = (subject: string, body: string) => {
    return api.post<Result<number, CreateTicketError>>('/api/tickets', {
      subject,
      body,
      userId: user.value.id,
    });
  };

  const addComment = (id: number, text: string) => {
    return api.post<Result<void, AddCommentToTicketError>>(`/api/tickets/${id}/comments`, {
      text,
      userId: user.value.id,
    });
  };

  const escalate = (id: number) => {
    return api.post<Result<void, EscalateTicketError>>(`/api/tickets/${id}/escalate`, null);
  };

  const solve = (id: number) => {
    return api.post<Result<void, SolveTicketError>>(`/api/tickets/${id}/solve`, null);
  };

  return {
    getAllTickets,
    getTicket,
    create,
    addComment,
    escalate,
    solve,
  };
};
