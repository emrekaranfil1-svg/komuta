import { api } from "./api.js";
import { portraitUrl } from "./portraits.js";

const ICONS = {
  amir: "◎",
  kriz: "◇",
  kurtarma: "⌁",
  sondurme: "♨",
  koruma: "⬡",
  ilkyardim: "✚",
  teknik: "⚙"
};

function visibleTeams() {
  return data?.teams || [];
}

const $ = s => document.querySelector(s);
const esc = s => String(s ?? "").replace(/[&<>"']/g, c => ({
  "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
}[c]));
function niceName(s) {
  return String(s ?? "")
    .toLocaleLowerCase("tr-TR")
    .replace(/(^|[\s./-])\S/g, c => c.toLocaleUpperCase("tr-TR"));
}
const time = s => new Date(s).toLocaleTimeString("tr-TR", { hour12: false });

let data = null;
let previousInside = new Set();
let firstLoad = true;
let busy = false;
let lastOk = 0;
let page = 0;
let renderSignature = "";
let toastTimer;

function toast(message) {
  const el = $("#toast");
  el.textContent = message;
  el.style.display = "block";
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => { el.style.display = "none"; }, 4000);
}

function connection(ok) {
  const el = $("#connection");
  if (!el) return;
  el.classList.toggle("stale", !ok);
  el.textContent = ok ? "" : `Bağlantı yok${lastOk ? " · son veri " + time(lastOk) : ""}`;
}

function tick() {
  $("#clock").textContent = time(Date.now());
  $("#date").textContent = new Date().toLocaleDateString("tr-TR", {
    day: "2-digit", month: "long", year: "numeric"
  }).toLocaleUpperCase("tr-TR");
  if (lastOk && Date.now() - lastOk > 12000) connection(false);
}

function count(id, n) {
  const el = $(id);
  const from = Number(el.dataset.value || 0);
  if (el.dataset.value === String(n)) return;
  el.dataset.value = n;
  if (matchMedia("(prefers-reduced-motion: reduce)").matches) {
    el.textContent = n.toLocaleString("tr-TR");
    return;
  }
  const start = performance.now();
  function frame(t) {
    if (Number(el.dataset.value) !== n) return;
    const f = Math.min((t - start) / 600, 1);
    el.textContent = Math.round(from + (n - from) * (1 - Math.pow(1 - f, 3))).toLocaleString("tr-TR");
    if (f < 1) requestAnimationFrame(frame);
  }
  requestAnimationFrame(frame);
}

function modeOf(state) {
  if (state.mode === "test" || state.mode === "emergency" || state.mode === "normal") return state.mode;
  return state.emergencyActive ? "emergency" : "normal";
}

function memberHtml(m) {
  const pulse = !firstLoad && !previousInside.has(m.id);
  return `<div class="member ${pulse ? "pulse" : ""}">
    <div class="avatar"><img src="${portraitUrl(m.avatarSeed, m.id)}" alt="" onerror="this.src='/assets/avatar-1.svg';this.onerror=null"></div>
    <div class="member-info">
      <div class="member-name">${esc(niceName(m.fullName))}</div>
      <div class="member-title">${esc(m.title)}</div>
    </div>
  </div>`;
}

function renderTeams() {
  const teams = visibleTeams();
  const sig = JSON.stringify(teams.map(t => [t.id, t.insideCount, t.totalCount, t.warn, t.members.map(m => [m.id, m.isInside])]));
  if (sig === renderSignature) return;
  renderSignature = sig;

  $("#teams").innerHTML = teams.map(team => {
    const inside = team.members.filter(m => m.isInside);
    const size = 4;
    const pages = Math.max(1, Math.ceil(inside.length / size));
    const p = page % pages;
    const visible = inside.length > size
      ? inside.slice(p * size, p * size + size)
      : inside;
    return `<article class="team-card ${team.warn ? "warning" : ""}">
      <div class="team-title">
        <span class="team-symbol">${ICONS[team.code] || "◆"}</span>
        <h3>${esc(team.name)}</h3>
        <span class="team-ratio"><b>${team.insideCount}</b>/${team.totalCount}</span>
      </div>
      <div class="team-progress"><span style="width:${team.totalCount ? team.insideCount / team.totalCount * 100 : 0}%"></span></div>
      <div class="members">${visible.map(memberHtml).join("") || '<span class="muted">İçeride üye yok</span>'}</div>
    </article>`;
  }).join("");
}

function renderEmergency() {
  const mode = modeOf(data);
  if (mode === "normal") return;
  const inside = data.remainingInside;
  const totalPages = Math.max(1, Math.ceil(inside.length / 9));
  const p = page % totalPages;
  $("#alarm-label").textContent = mode === "test" ? "SİMÜLASYON · ACİL DURUM TESTİ" : "ACİL DURUM / TAHLİYE MODU";
  $("#remaining-count").textContent = inside.length;
  $("#evac-page").textContent = `Sayfa ${p + 1}/${totalPages} · 8 sn otomatik`;
  $("#evac-list").innerHTML = inside.slice(p * 9, p * 9 + 9).map(x =>
    `<div class="evac-person">${esc(niceName(x.fullName))}<small>${esc(x.title)}</small></div>`
  ).join("") || '<p class="green">İçeride görünen kişi yok.</p>';
  $("#responders").innerHTML = visibleTeams().map(t => {
    const names = t.members.filter(m => m.isInside).map(m => esc(niceName(m.fullName))).join(", ");
    return `<div class="responder-row"><b>${esc(t.name)}</b><span>${names || "İçeride üye yok"}</span></div>`;
  }).join("");
}

function apply() {
  const mode = modeOf(data);
  document.body.classList.toggle("alarm", mode !== "normal");
  $("#normal-stage").hidden = mode !== "normal";
  $("#emergency-stage").hidden = mode === "normal";

  count("#personnel-count", data.hud.insideStaff);
  count("#visitor-count", data.hud.visitorsContractors);
  count("#total-count", data.hud.totalInside);
  $("#safe-days").textContent = data.accidentFreeDays;

  renderTeams();
  renderEmergency();

  previousInside = new Set(visibleTeams().flatMap(t => t.members.filter(m => m.isInside).map(m => m.id)));
  firstLoad = false;
}

async function load() {
  if (busy) return;
  busy = true;
  try {
    data = await api.dashboard();
    lastOk = Date.now();
    connection(true);
    apply();
  } catch {
    connection(false);
  } finally {
    busy = false;
  }
}

$("#fullscreen").onclick = () => {
  (document.fullscreenElement ? document.exitFullscreen() : document.documentElement.requestFullscreen())
    .catch(() => toast("Tam ekran bu tarayıcıda açılamadı."));
};

tick();
setInterval(tick, 1000);
load();
setInterval(load, 2000);
setInterval(() => {
  page++;
  renderSignature = "";
  if (data) {
    renderTeams();
    renderEmergency();
  }
}, 8000);
