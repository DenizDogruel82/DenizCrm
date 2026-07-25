const token = sessionStorage.getItem("accessToken");
const notification = document.querySelector("#notification");
let customers = [];
let activities = [];
let leads = [];
let opportunities = [];
let stages = [];

if (!token) {
  window.location.replace("/");
} else {
  initialize();
}

async function api(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
      ...options.headers
    }
  });

  if (response.status === 401) {
    logout();
    throw new Error("Oturum süresi doldu.");
  }
  if (!response.ok) {
    const problem = await response.json().catch(() => ({}));
    throw new Error(problem.detail || problem.title || "API isteği başarısız.");
  }
  return response.status === 204 ? null : response.json();
}

async function initialize() {
  try {
    const [user, customerResult, activityResult] = await Promise.all([
      api("/api/auth/me"),
      api("/api/customers?pageSize=100"),
      api("/api/activities")
    ]);
    const name = user.fullName || user.email || "Kullanıcı";
    document.querySelector("#userName").textContent = name;
    document.querySelector("#avatar").textContent = name.charAt(0).toLocaleUpperCase("tr-TR");
    sessionStorage.setItem("currentUser", JSON.stringify(user));
    customers = customerResult.items;
    activities = activityResult;
    renderAll();
    await loadExtendedData(user);
  } catch (error) {
    notify(error.message, true);
  }
}

async function safeApi(path, fallback) {
  try { return await api(path); }
  catch (error) {
    console.warn(`${path}: ${error.message}`);
    return fallback;
  }
}

async function loadExtendedData(user) {
  const [leadResult, opportunityResult, stageResult, insights, users, subscription, tenant] = await Promise.all([
    safeApi("/api/leads?pageSize=100", { items: [] }),
    safeApi("/api/opportunities?pageSize=100", { items: [] }),
    safeApi("/api/pipeline-stages", []),
    safeApi("/api/ai-insights", []),
    safeApi("/api/users", []),
    safeApi("/api/subscriptions/current", null),
    user.tenantId ? safeApi(`/api/tenants/${user.tenantId}`, null) : null
  ]);
  leads = leadResult.items;
  opportunities = opportunityResult.items;
  stages = stageResult;
  renderExtended(insights, users, subscription, tenant);
}

function renderExtended(insights, users, subscription, tenant) {
  const leadStatuses = ["", "Yeni", "İletişime geçildi", "Nitelikli", "Niteliksiz", "Dönüştürüldü", "Kaybedildi"];
  document.querySelector("#opportunityCount").textContent = opportunities.length;
  document.querySelector("#leadRows").innerHTML = tableEmptyOr(leads, lead => `
    <tr><td><strong>${escapeHtml(`${lead.firstName} ${lead.lastName}`)}</strong></td><td>${escapeHtml(lead.companyName || "—")}</td>
    <td>${escapeHtml(lead.email || lead.phone || "—")}</td><td>${leadStatuses[lead.status] || lead.status}</td><td>${lead.score}</td></tr>`, 5);
  document.querySelector("#opportunityRows").innerHTML = tableEmptyOr(opportunities, opportunity => {
    const customer = customers.find(item => item.id === opportunity.customerId);
    const stage = stages.find(item => item.id === opportunity.pipelineStageId);
    return `<tr><td><strong>${escapeHtml(opportunity.title)}</strong></td><td>${escapeHtml(customer?.name || "—")}</td>
      <td>${escapeHtml(stage?.name || "—")}</td><td>${Number(opportunity.amount).toLocaleString("tr-TR")} ${escapeHtml(opportunity.currency)}</td><td>%${opportunity.probability}</td></tr>`;
  }, 5);
  renderCards("#aiCards", insights, insight => `<div class="data-card"><span class="badge">AI · ${insight.type}</span><h3>${escapeHtml(insight.title)}</h3><p>${escapeHtml(insight.content)}</p><small>${escapeHtml(insight.model)}${insight.confidence != null ? ` · Güven %${Math.round(insight.confidence * 100)}` : ""}</small></div>`);
  renderCards("#userCards", users, user => `<div class="data-card"><h3>${escapeHtml(user.fullName)}</h3><p>${escapeHtml(user.email)}</p><span class="badge">${escapeHtml(user.role)}</span></div>`);
  renderCards("#stageCards", stages, stage => `<div class="data-card"><h3><span style="color:${escapeHtml(stage.color)}">●</span> ${escapeHtml(stage.name)}</h3><p>Sıra ${stage.order} · Kazanma olasılığı %${stage.winProbability}</p></div>`);
  const accounts = [];
  if (tenant) accounts.push(`<div class="data-card"><h3>${escapeHtml(tenant.name)}</h3><p>${escapeHtml(tenant.slug)} · ${escapeHtml(tenant.currency)} · ${escapeHtml(tenant.timeZone)}</p></div>`);
  if (subscription) accounts.push(`<div class="data-card"><h3>${escapeHtml(subscription.planCode)}</h3><p>Koltuk limiti: ${subscription.seatLimit} · Durum: ${subscription.status}</p><small>${new Date(subscription.periodEndUtc).toLocaleDateString("tr-TR")} tarihine kadar</small></div>`);
  document.querySelector("#accountCards").innerHTML = accounts.join("") || `<div class="empty-state">Abonelik bilgisi bulunamadı.</div>`;
  document.querySelector("#relatedCustomer").innerHTML = `<option value="">Müşteri seçin</option>${customers.map(customer => `<option value="${customer.id}">${escapeHtml(customer.name)}</option>`).join("")}`;
}

function tableEmptyOr(items, template, columns) {
  return items.length ? items.map(template).join("") : `<tr><td class="empty-state" colspan="${columns}">Kayıt bulunamadı.</td></tr>`;
}

function renderCards(selector, items, template) {
  document.querySelector(selector).innerHTML = items.length ? items.map(template).join("") : `<div class="empty-state">Kayıt bulunamadı.</div>`;
}

function renderAll() {
  document.querySelector("#customerCount").textContent = customers.length;
  document.querySelector("#activityCount").textContent = activities.length;
  renderCustomers(customers);
  renderActivities();
  document.querySelector("#activityCustomer").innerHTML =
    `<option value="">Müşteri seçilmedi</option>${customers.map(customer =>
      `<option value="${customer.id}">${escapeHtml(customer.name)}</option>`).join("")}`;
}

function renderCustomers(items) {
  const rows = document.querySelector("#customerRows");
  rows.innerHTML = items.length ? items.map(customer => `
    <tr>
      <td><strong>${escapeHtml(customer.name)}</strong></td>
      <td>${customer.type === 2 ? "Şirket" : "Bireysel"}</td>
      <td>${escapeHtml(customer.email || customer.phone || "—")}</td>
      <td>${escapeHtml(customer.city || "—")}</td>
      <td><button class="danger" data-delete-customer="${customer.id}">Sil</button></td>
    </tr>`).join("") : `<tr><td class="empty-state" colspan="5">Müşteri bulunamadı.</td></tr>`;
}

function renderActivities() {
  const typeNames = ["", "Arama", "E-posta", "Toplantı", "Görev", "Demo"];
  const statusNames = ["", "Planlandı", "Tamamlandı", "İptal"];
  document.querySelector("#activityCards").innerHTML = activities.length ? activities.map(activity => `
    <article class="activity-card">
      <div>
        <span class="badge">${typeNames[activity.type] || activity.type}</span>
        <h3>${escapeHtml(activity.subject)}</h3>
        <p>${escapeHtml(activity.description || "Açıklama yok")}</p>
        <small>${statusNames[activity.status] || activity.status}${activity.dueAtUtc ? ` · ${new Date(activity.dueAtUtc).toLocaleString("tr-TR")}` : ""}</small>
      </div>
      <div class="card-actions">
        ${activity.status === 1 ? `<button class="secondary" data-complete-activity="${activity.id}">Tamamla</button>` : ""}
        <button class="danger" data-delete-activity="${activity.id}">Sil</button>
      </div>
    </article>`).join("") : `<div class="empty-state">Aktivite bulunamadı.</div>`;
}

function notify(message, error = false) {
  notification.textContent = message;
  notification.className = `notification${error ? " error" : ""}`;
  setTimeout(() => { notification.textContent = ""; }, 4000);
}

function escapeHtml(value) {
  const element = document.createElement("span");
  element.textContent = value;
  return element.innerHTML;
}

function logout() {
  sessionStorage.removeItem("accessToken");
  sessionStorage.removeItem("currentUser");
  window.location.replace("/");
}

document.querySelector("#logoutButton").addEventListener("click", logout);

document.querySelectorAll(".nav-link").forEach(button => {
  button.addEventListener("click", () => {
    document.querySelectorAll(".nav-link, .view").forEach(element => element.classList.remove("active"));
    button.classList.add("active");
    document.querySelector(`#${button.dataset.view}View`).classList.add("active");
  });
});

document.querySelector("#showCustomerForm").addEventListener("click", () => document.querySelector("#customerForm").classList.remove("hidden"));
document.querySelector("#cancelCustomer").addEventListener("click", () => document.querySelector("#customerForm").classList.add("hidden"));
document.querySelector("#showActivityForm").addEventListener("click", () => document.querySelector("#activityForm").classList.remove("hidden"));
document.querySelector("#cancelActivity").addEventListener("click", () => document.querySelector("#activityForm").classList.add("hidden"));

document.querySelector("#customerSearch").addEventListener("input", event => {
  const search = event.target.value.toLocaleLowerCase("tr-TR");
  renderCustomers(customers.filter(customer =>
    [customer.name, customer.email, customer.city].some(value => value?.toLocaleLowerCase("tr-TR").includes(search))));
});

document.querySelector("#relatedCustomer").addEventListener("change", async event => {
  const customerId = event.target.value;
  if (!customerId) {
    renderCards("#contactCards", [], () => "");
    renderCards("#noteCards", [], () => "");
    return;
  }
  const [contacts, notes] = await Promise.all([
    safeApi(`/api/contacts/customer/${customerId}`, []),
    safeApi(`/api/notes/customer/${customerId}`, [])
  ]);
  renderCards("#contactCards", contacts, contact => `<div class="data-card"><h3>${escapeHtml(`${contact.firstName} ${contact.lastName}`)}</h3><p>${escapeHtml(contact.jobTitle || "Görev belirtilmedi")}</p><small>${escapeHtml(contact.email || contact.phone || "İletişim bilgisi yok")}</small></div>`);
  renderCards("#noteCards", notes, note => `<div class="data-card"><p>${escapeHtml(note.content)}</p><small>${new Date(note.createdAtUtc).toLocaleString("tr-TR")}</small></div>`);
});

document.querySelector("#customerForm").addEventListener("submit", async event => {
  event.preventDefault();
  const form = new FormData(event.currentTarget);
  try {
    const customer = await api("/api/customers", {
      method: "POST",
      body: JSON.stringify({
        name: form.get("name"), type: Number(form.get("type")),
        email: form.get("email") || null, phone: form.get("phone") || null,
        industry: form.get("industry") || null, city: form.get("city") || null,
        website: null, taxNumber: null, address: null, country: null, ownerUserId: null
      })
    });
    customers.unshift(customer);
    event.currentTarget.reset();
    event.currentTarget.classList.add("hidden");
    renderAll();
    notify("Müşteri başarıyla eklendi.");
  } catch (error) { notify(error.message, true); }
});

document.querySelector("#activityForm").addEventListener("submit", async event => {
  event.preventDefault();
  const form = new FormData(event.currentTarget);
  try {
    const activity = await api("/api/activities", {
      method: "POST",
      body: JSON.stringify({
        subject: form.get("subject"), description: form.get("description") || null,
        type: Number(form.get("type")), customerId: form.get("customerId") || null,
        contactId: null, leadId: null, opportunityId: null, assignedUserId: null,
        dueAtUtc: form.get("dueAtUtc") ? new Date(form.get("dueAtUtc")).toISOString() : null
      })
    });
    activities.unshift(activity);
    event.currentTarget.reset();
    event.currentTarget.classList.add("hidden");
    renderAll();
    notify("Aktivite başarıyla eklendi.");
  } catch (error) { notify(error.message, true); }
});

document.addEventListener("click", async event => {
  const customerId = event.target.dataset.deleteCustomer;
  const activityId = event.target.dataset.deleteActivity;
  const completeId = event.target.dataset.completeActivity;
  try {
    if (customerId) {
      await api(`/api/customers/${customerId}`, { method: "DELETE" });
      customers = customers.filter(customer => customer.id !== customerId);
      notify("Müşteri silindi.");
    } else if (activityId) {
      await api(`/api/activities/${activityId}`, { method: "DELETE" });
      activities = activities.filter(activity => activity.id !== activityId);
      notify("Aktivite silindi.");
    } else if (completeId) {
      const updated = await api(`/api/activities/${completeId}/complete`, { method: "POST" });
      activities = activities.map(activity => activity.id === completeId ? updated : activity);
      notify("Aktivite tamamlandı.");
    } else return;
    renderAll();
  } catch (error) { notify(error.message, true); }
});
