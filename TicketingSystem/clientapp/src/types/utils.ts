export type Option<T> = Some<T> | None;

type Some<T> = {
  hasValue: true;
  value: T;
};

type None = {
  hasValue: false;
};

export type Result<TOk, TErr> = OkResult<TOk> | ErrResult<TErr>;

type OkResult<TOk> = {
  isOk: true;
  ok: TOk;
};

type ErrResult<TErr> = {
  isOk: false;
  err: TErr;
};

export type Union<Tag extends string, T> = {
  tag: Tag;
  value: T;
};

type Handlers<U extends { tag: string; value: unknown }, R> = {
  [K in U['tag']]: (value: Extract<U, { tag: K }>['value']) => R;
};

export const handleUnion = <U extends { tag: string; value: unknown }, R>(
  handlers: Handlers<U, R>,
  union: U,
) => {
  const handler = handlers[union.tag as U['tag']];
  return handler(union.value);
};
