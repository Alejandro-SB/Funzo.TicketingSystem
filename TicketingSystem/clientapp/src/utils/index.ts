const dateFormatter = new Intl.DateTimeFormat(undefined, {
  dateStyle: 'full',
  timeStyle: 'long',
});
export const toParam = (v: string | string[] | undefined) => (Array.isArray(v) ? v[0] : v);

export const formatDate = (d: string) => dateFormatter.format(new Date(d));
