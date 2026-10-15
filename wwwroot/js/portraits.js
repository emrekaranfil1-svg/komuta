export function portraitUrl(seed = 1, id = 1) {
  const n = (Math.abs(Number(seed) || Number(id) || 1) % 23) + 1;
  return `/assets/avatar-${n}.svg`;
}
