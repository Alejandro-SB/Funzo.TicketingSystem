export const get = (url: string) => {
  return fetch(url).then((r) => r.json());
};
