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

export type Err<TErr extends string, TErrP> = Record<TErr, TErrP>;

// export type UnionToFunc<U extends Err<TErr, TErrP>, TErr extends string, TErrP, TOut> = {
//   readonly [key in keyof U]: (x: TErrP) => TOut;
// };

export type UnionToFunc<U, TOut> =
  U extends Err<infer _, infer TPayload>
    ? {
        readonly [key in keyof U]: (x: TPayload) => TOut;
      }
    : never;
