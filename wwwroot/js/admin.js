import { api } from "./api.js";

const $ = s => document.querySelector(s);
const esc = s => String(s ?? "").replace(/[&<>"']/g, c => ({
  "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
}[c]));
const time = s => new Date(s).toLocaleTimeString("tr-TR", { hour12: false });
const kindLabel = k => ({ employee: "Çalışan", visitor: "Ziyaretçi", contractor: "Taşeron" }[k] || k);

let people = [];
let teams = [];
let lastSignature = "";
let lastOk = 0;
let toastTimer;
let pendingDelete = null;

function sessionUser() {
  try { return JSON.parse(localStorage.getItem("komuta-user") || "null"); }
  catch { return null; }
}

function toast(message) {
  const el = $("#toast");
  el.textContent = message;
  el.style.display = "block";
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => { el.style.display = "none"; }, 4000);
}

function tick() {
  $("#clock").textContent = time(Date.now());
  $("#date").textContent = new Date().toLocaleDateString("tr-TR", {
    day: "2-digit", month: "long", year: "numeric"
  }).toLocaleUpperCase("tr-TR");
}

function dialogOpen() {
  return Boolean(document.querySelector("dialog[open]"));
}

function selectedTeamIds(root) {
  return [...root.querySelectorAll("input[data-team]:checked")].map(c => Number(c.dataset.team));
}

function renderTeams() {
  $("#team-summary").textContent = `${teams.length} ekip türü`;
  $("#team-catalog").innerHTML = teams.map(t => `<article class="team-edit-card">
    <div>
      <h3>${esc(t.name)}</h3>
      <small>${t.isCritical ? "Kritik ekip" : "Standart ekip"} · ${people.filter(p => p.teamIds.includes(t.id)).length} kişi</small>
    </div>
    <div class="row-actions">
      <button type="button" data-edit-team="${t.id}">Düzenle</button>
      <button type="button" class="danger-button" data-del-team="${t.id}">Sil</button>
    </div>
  </article>`).join("") || `<p class="empty">Henüz ekip türü yok. Yeni ekip türü ile başlayın.</p>`;
}

function renderTable(force = false) {
  const query = $("#search").value.toLocaleLowerCase("tr-TR");
  const filter = $("#filter").value;
  const rows = people.filter(p => {
    const hay = (p.fullName + " " + p.title + " " + p.cardId).toLocaleLowerCase("tr-TR");
    if (query && !hay.includes(query)) return false;
    if (filter === "inside") return p.isInside;
    if (filter === "outside") return !p.isInside;
    if (filter === "visitor") return p.kind === "visitor" || p.kind === "contractor";
    return true;
  });
  $("#person-summary").textContent = `${rows.length} kişi listeleniyor · ${people.filter(p => p.isInside).length} kişi fabrikada`;
  const head = $("#team-head");
  if (head) {
    head.innerHTML = `<th>PERSONEL</th><th>TESİS DURUMU</th>${teams.map(t => `<th>${esc(t.name).toLocaleUpperCase("tr-TR")}</th>`).join("")}<th>İŞLEM</th>`;
  }
  const sig = JSON.stringify([rows, query, filter, teams.map(t => t.id)]);
  if (!force && sig === lastSignature) return;
  lastSignature = sig;
  const cols = 3 + teams.length;
  $("#person-table").innerHTML = rows.map(p => `<tr>
    <td>${esc(p.fullName)}${p.isVip ? " · VIP" : ""}<small>${esc(p.title)} · ${esc(kindLabel(p.kind))} · ${esc(p.cardId)}</small></td>
    <td>
      <button class="presence-toggle ${p.isInside ? "" : "out"}" data-presence="${p.id}" type="button" aria-label="${esc(p.fullName)}: ${p.isInside ? "fabrikadan çıkar" : "fabrikaya al"}">
        ${p.isInside ? "● İçeride →" : "○ Dışarıda →"}
      </button>
    </td>
    ${teams.map(t => `<td><input type="checkbox" data-person="${p.id}" data-team="${t.id}" ${p.teamIds.includes(t.id) ? "checked" : ""} aria-label="${esc(p.fullName)} · ${esc(t.name)}"></td>`).join("")}
    <td>
      <div class="row-actions">
        <button type="button" data-edit-person="${p.id}">Düzenle</button>
        <button type="button" class="danger-button" data-del-person="${p.id}">Sil</button>
      </div>
    </td>
  </tr>`).join("") || `<tr><td colspan="${cols}" class="empty">Aramanızla eşleşen personel bulunamadı.</td></tr>`;
}

function fillPersonTeams(selected) {
  const ids = new Set(selected || []);
  $("#person-teams").innerHTML = teams.map(t => `<label>
    <input type="checkbox" data-team="${t.id}" ${ids.has(t.id) ? "checked" : ""}>
    ${esc(t.name)}
  </label>`).join("") || `<span class="muted">Önce bir ekip türü oluşturun.</span>`;
}

function openPerson(person) {
  $("#person-id").value = person?.id || "";
  $("#person-eyebrow").textContent = person ? "KAYDI DÜZENLE" : "YENİ KAYIT";
  $("#person-title").textContent = person ? "Kişiyi düzenle" : "Kişi ekle";
  $("#person-name").value = person?.fullName || "";
  $("#person-job").value = person?.title || "";
  $("#person-kind").value = person?.kind || "employee";
  $("#person-card").value = person?.cardId || "";
  $("#person-inside").checked = Boolean(person?.isInside);
  $("#person-vip").checked = Boolean(person?.isVip);
  fillPersonTeams(person?.teamIds || []);
  $("#person-dialog").showModal();
  $("#person-name").focus();
}

function openTeam(team) {
  $("#team-id").value = team?.id || "";
  $("#team-eyebrow").textContent = team ? "EKİBİ DÜZENLE" : "YENİ EKİP";
  $("#team-title").textContent = team ? "Ekip türünü düzenle" : "Ekip türü oluştur";
  $("#team-name").value = team?.name || "";
  $("#team-critical").checked = Boolean(team?.isCritical);
  $("#team-dialog").showModal();
  $("#team-name").focus();
}

function confirmDelete(kind, id, title, copy) {
  pendingDelete = { kind, id };
  $("#confirm-title").textContent = title;
  $("#confirm-copy").textContent = copy;
  $("#confirm-dialog").showModal();
}

async function load() {
  if (!sessionUser() || dialogOpen()) return;
  try {
    const [list, teamList] = await Promise.all([api.personnel(), api.teams()]);
    people = list;
    teams = teamList;
    lastOk = Date.now();
    renderTeams();
    renderTable();
  } catch {
    if (lastOk) toast("Bağlantı yok, son veri gösteriliyor.");
  }
}

async function safeAction(fn, message) {
  try {
    await fn();
    toast(message || "Güncellendi.");
    lastSignature = "";
    await load();
  } catch (e) {
    toast(e.message);
    lastSignature = "";
    await load();
    renderTable(true);
  }
}

function requireAuth() {
  if (!sessionUser()) $("#login-dialog").showModal();
}

$("#fullscreen").onclick = () => {
  (document.fullscreenElement ? document.exitFullscreen() : document.documentElement.requestFullscreen())
    .catch(() => toast("Tam ekran bu tarayıcıda açılamadı."));
};

$("#search").oninput = () => renderTable();
$("#filter").onchange = () => renderTable();

$("#add-person").onclick = () => openPerson(null);
$("#add-team").onclick = () => openTeam(null);
$("#person-cancel").onclick = () => $("#person-dialog").close();
$("#team-cancel").onclick = () => $("#team-dialog").close();

$("#person-table").onclick = e => {
  const edit = e.target.closest("[data-edit-person]");
  if (edit) {
    openPerson(people.find(p => p.id === Number(edit.dataset.editPerson)));
    return;
  }
  const del = e.target.closest("[data-del-person]");
  if (del) {
    const person = people.find(p => p.id === Number(del.dataset.delPerson));
    confirmDelete("person", person.id, "Kişi silinsin mi?", `${person.fullName} kayıtlardan kaldırılacak.`);
    return;
  }
  const b = e.target.closest("[data-presence]");
  if (!b) return;
  const person = people.find(p => p.id === Number(b.dataset.presence));
  b.disabled = true;
  safeAction(() => api.toggle(person.id, !person.isInside));
};

$("#person-table").onchange = e => {
  const c = e.target;
  if (!c.dataset.team) return;
  const person = people.find(p => p.id === Number(c.dataset.person));
  const teamId = Number(c.dataset.team);
  const teamIds = c.checked
    ? [...new Set([...person.teamIds, teamId])]
    : person.teamIds.filter(id => id !== teamId);
  c.disabled = true;
  safeAction(() => api.assignTeams(person.id, teamIds));
};

$("#team-catalog").onclick = e => {
  const edit = e.target.closest("[data-edit-team]");
  if (edit) {
    openTeam(teams.find(t => t.id === Number(edit.dataset.editTeam)));
    return;
  }
  const del = e.target.closest("[data-del-team]");
  if (!del) return;
  const team = teams.find(t => t.id === Number(del.dataset.delTeam));
  confirmDelete("team", team.id, "Ekip türü silinsin mi?", `${team.name} kaldırılacak. Kişiler durur, yalnızca bu ekip görevi silinir.`);
};

$("#person-form").onsubmit = async e => {
  e.preventDefault();
  const id = Number($("#person-id").value);
  const payload = {
    fullName: $("#person-name").value.trim(),
    title: $("#person-job").value.trim(),
    kind: $("#person-kind").value,
    cardId: $("#person-card").value.trim(),
    isInside: $("#person-inside").checked,
    isVip: $("#person-vip").checked,
    teamIds: selectedTeamIds($("#person-teams"))
  };
  try {
    if (id) await api.updatePerson(id, payload);
    else await api.savePerson(payload);
    $("#person-dialog").close();
    toast(id ? "Kişi güncellendi." : "Kişi eklendi.");
    lastSignature = "";
    await load();
  } catch (err) {
    toast(err.message);
  }
};

$("#team-form").onsubmit = async e => {
  e.preventDefault();
  const id = Number($("#team-id").value);
  const payload = {
    name: $("#team-name").value.trim(),
    minInside: 0,
    isCritical: $("#team-critical").checked
  };
  try {
    if (id) await api.updateTeam(id, payload);
    else await api.saveTeam(payload);
    $("#team-dialog").close();
    toast(id ? "Ekip güncellendi." : "Ekip oluşturuldu.");
    lastSignature = "";
    await load();
  } catch (err) {
    toast(err.message);
  }
};

$("#confirm-dialog").addEventListener("close", () => {
  if ($("#confirm-dialog").returnValue !== "confirm" || !pendingDelete) return;
  const { kind, id } = pendingDelete;
  pendingDelete = null;
  safeAction(
    () => kind === "person" ? api.deletePerson(id) : api.deleteTeam(id),
    kind === "person" ? "Kişi silindi." : "Ekip silindi."
  );
});

$("#login-dialog").addEventListener("cancel", e => e.preventDefault());
$("#login-form").onsubmit = async e => {
  e.preventDefault();
  $("#login-error").textContent = "";
  try {
    const user = await api.login({
      username: $("#login-user").value.trim(),
      password: $("#login-pass").value
    });
    localStorage.setItem("komuta-user", JSON.stringify(user));
    $("#login-pass").value = "";
    $("#login-dialog").close();
    await load();
  } catch {
    $("#login-error").textContent = "Giriş reddedildi.";
  }
};

$("#logout").onclick = () => {
  localStorage.removeItem("komuta-user");
  location.href = "/";
};

tick();
setInterval(tick, 1000);
requireAuth();
if (sessionUser()) load();
setInterval(() => { if (sessionUser()) load(); }, 2000);
