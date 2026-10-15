async function request(path, options = {}) {
  const response = await fetch(path, {
    cache: "no-store",
    headers: { "Content-Type": "application/json", ...(options.headers || {}) },
    ...options
  });

  if (!response.ok) {
    const raw = (await response.text()).trim();
    let message = raw;
    try {
      const json = JSON.parse(raw);
      if (typeof json.message === "string") message = json.message;
    } catch {
      /* düz metin */
    }
    throw new Error(message ? `${response.status} — ${message}` : `${response.status} ${response.statusText}`);
  }

  if (response.status === 204) return null;
  const text = await response.text();
  return text ? JSON.parse(text) : null;
}

export const api = {
  login: payload => request("/api/auth/login", { method: "POST", body: JSON.stringify(payload) }),
  dashboard: () => request("/api/dashboard"),
  personnel: () => request("/api/personnel"),
  teams: () => request("/api/teams"),
  toggle: (id, inside) => request(`/api/personnel/${id}/toggle`, { method: "POST", body: JSON.stringify({ inside }) }),
  assignTeams: (id, teamIds) => request(`/api/personnel/${id}/teams`, { method: "PUT", body: JSON.stringify({ teamIds }) }),
  savePerson: payload => request("/api/personnel", { method: "POST", body: JSON.stringify(payload) }),
  updatePerson: (id, payload) => request(`/api/personnel/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deletePerson: id => request(`/api/personnel/${id}`, { method: "DELETE" }),
  saveTeam: payload => request("/api/teams", { method: "POST", body: JSON.stringify(payload) }),
  updateTeam: (id, payload) => request(`/api/teams/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  deleteTeam: id => request(`/api/teams/${id}`, { method: "DELETE" }),
  swipe: cardId => request("/api/turnstile/swipe", { method: "POST", body: JSON.stringify({ cardId }) }),
  randomSwipe: () => request("/api/turnstile/random", { method: "POST" })
};
