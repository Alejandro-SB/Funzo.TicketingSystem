import { useApi } from './api';
import type { Result } from '../types/utils';
import { type Option } from '../types/utils';
import { useAuth } from 'src/composables/useAuth';
export type GetAllTicketsResponse = {
  tickets: GetAllTicketsTicket[];
};
export type GetAllTicketsTicket = {
  id: number;
  subject: string;
  body: string;
  totalComments: number;
};
export type GetTicketResponse = {
  id: number;
  subject: string;
  body: string;
  comments: {
    userId: number;
    displayName: string;
    text: string;
  };
};

export type TicketNotFoundError = {
  TicketNotFound: object;
};

export type UserNotFoundError = {
  UserNotFound: object;
};

export type InvalidCommentText = {
  InvalidCommentText: object;
};

export type TicketAlreadySolved = {
  TicketAlreadySolved: {
    resolutionDate: string;
  };
};

export type TicketAlreadyEscalated = {
  TicketAlreadyEscalated: object;
};

export type InvalidTicketBodyError = {
  InvalidTicketBody: {
    reason: string;
  };
};

export type InvalidTicketSubjectError = {
  InvalidTicketSubject: {
    reason: string;
  };
};

type AddCommentToTicketError =
  | TicketNotFoundError
  | UserNotFoundError
  | InvalidCommentText
  | TicketAlreadySolved;

type EscalateTicketError = TicketAlreadyEscalated | TicketAlreadySolved | TicketNotFoundError;

type SolveTicketError = TicketNotFoundError | TicketAlreadySolved;

type CreateTicketError = UserNotFoundError | InvalidTicketBodyError | InvalidTicketSubjectError;

export const useTicketsApi = () => {
  const api = useApi();
  const { userId } = useAuth();

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
      userId: userId.value,
    });
  };

  const addComment = (id: number, text: string) => {
    return api.post<Result<void, AddCommentToTicketError>>(`/api/tickets/${id}/comments`, {
      text,
      userId: userId.value,
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
