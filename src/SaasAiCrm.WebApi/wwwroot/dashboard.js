const token = sessionStorage.getItem("accessToken");
const notification = document.querySelector("#notification");
let customers = [];
let activities = [];
let leads = [];
let opportunities = [];
let stages = [];
let insights = [];
let users = [];
let subscription = null;
let tenant = null;
let currentUser = null;
let selectedContacts = [];
let selectedNotes = [];
let dialogSubmit = null;

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
    currentUser = user;
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
  const [leadResult, opportunityResult, stageResult, insightResult, userResult, subscriptionResult, tenantResult] = await Promise.all([
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
  insights = insightResult;
  users = userResult;
  subscription = subscriptionResult;
  tenant = tenantResult;
  document.querySelector(".admin-link").classList.toggle("hidden", user.role !== "Admin");
  renderExtended();
}

function renderExtended() {
  const leadStatuses = ["", "Yeni", "İletişime geçildi", "Nitelikli", "Niteliksiz", "Dönüştürüldü", "Kaybedildi"];
  document.querySelector("#opportunityCount").textContent = opportunities.length;
  document.querySelector("#leadRows").innerHTML = tableEmptyOr(leads, lead => `
    <tr><td><strong>${escapeHtml(`${lead.firstName} ${lead.lastName}`)}</strong></td><td>${escapeHtml(lead.companyName || "—")}</td>
    <td>${escapeHtml(lead.email || lead.phone || "—")}</td><td>${leadStatuses[lead.status] || lead.status}</td><td>${lead.score}
    <div class="row-actions"><button class="secondary mini" data-edit-lead="${lead.id}">Düzenle</button>${lead.status !== 5 ? `<button class="secondary mini" data-convert-lead="${lead.id}">Dönüştür</button>` : ""}<button class="danger" data-delete-lead="${lead.id}">Sil</button></div></td></tr>`, 5);
  document.querySelector("#opportunityRows").innerHTML = tableEmptyOr(opportunities, opportunity => {
    const customer = customers.find(item => item.id === opportunity.customerId);
    const stage = stages.find(item => item.id === opportunity.pipelineStageId);
    return `<tr><td><strong>${escapeHtml(opportunity.title)}</strong></td><td>${escapeHtml(customer?.name || "—")}</td>
      <td>${escapeHtml(stage?.name || "—")}</td><td>${Number(opportunity.amount).toLocaleString("tr-TR")} ${escapeHtml(opportunity.currency)}</td><td>%${opportunity.probability}
      <div class="row-actions"><button class="secondary mini" data-edit-opportunity="${opportunity.id}">Düzenle</button><button class="secondary mini" data-stage-opportunity="${opportunity.id}">Aşama değiştir</button><button class="danger" data-delete-opportunity="${opportunity.id}">Sil</button></div></td></tr>`;
  }, 5);
  renderCards("#aiCards", insights, insight => `<div class="data-card"><span class="badge">AI · ${insight.type}</span><h3>${escapeHtml(insight.title)}</h3><p>${escapeHtml(insight.content)}</p><small>${escapeHtml(insight.model)}${insight.confidence != null ? ` · Güven %${Math.round(insight.confidence * 100)}` : ""}</small><div class="row-actions"><button class="secondary mini" data-dismiss-insight="${insight.id}">${insight.isDismissed ? "Geri al" : "Kapat"}</button><button class="danger" data-delete-insight="${insight.id}">Sil</button></div></div>`);
  renderCards("#userCards", users, user => `<div class="data-card"><h3>${escapeHtml(user.fullName)}</h3><p>${escapeHtml(user.email)}</p><span class="badge">${escapeHtml(user.role)}</span><div class="row-actions"><button class="secondary mini" data-edit-user="${user.id}">Düzenle</button><button class="danger" data-delete-user="${user.id}">Sil</button></div></div>`);
  renderCards("#stageCards", stages, stage => `<div class="data-card"><h3><span style="color:${escapeHtml(stage.color)}">●</span> ${escapeHtml(stage.name)}</h3><p>Sıra ${stage.order} · Kazanma olasılığı %${stage.winProbability}</p><div class="row-actions"><button class="secondary mini" data-edit-stage="${stage.id}">Düzenle</button><button class="danger" data-delete-stage="${stage.id}">Sil</button></div></div>`);
  const accounts = [];
  if (tenant) accounts.push(`<div class="data-card"><h3>${escapeHtml(tenant.name)}</h3><p>${escapeHtml(tenant.slug)} · ${escapeHtml(tenant.currency)} · ${escapeHtml(tenant.timeZone)}</p><div class="row-actions"><button class="secondary mini" data-edit-tenant="${tenant.id}">Düzenle</button></div></div>`);
  if (subscription) accounts.push(`<div class="data-card"><h3>${escapeHtml(subscription.planCode)}</h3><p>Koltuk limiti: ${subscription.seatLimit} · Durum: ${subscription.status}</p><small>${new Date(subscription.periodEndUtc).toLocaleDateString("tr-TR")} tarihine kadar</small><div class="row-actions"><button class="secondary mini" data-edit-subscription="${subscription.id}">Düzenle</button><button class="danger" data-cancel-subscription="${subscription.id}">İptal et</button></div></div>`);
  else if (currentUser?.role === "Admin") accounts.push(`<button class="primary" id="newSubscription" type="button">Abonelik oluştur</button>`);
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
      <td><div class="row-actions"><button class="secondary mini" data-edit-customer="${customer.id}">Düzenle</button><button class="danger" data-delete-customer="${customer.id}">Sil</button></div></td>
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
        <button class="secondary" data-edit-activity="${activity.id}">Düzenle</button>
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
  selectedContacts = contacts;
  selectedNotes = notes;
  renderCards("#contactCards", contacts, contact => `<div class="data-card"><h3>${escapeHtml(`${contact.firstName} ${contact.lastName}`)}</h3><p>${escapeHtml(contact.jobTitle || "Görev belirtilmedi")}</p><small>${escapeHtml(contact.email || contact.phone || "İletişim bilgisi yok")}</small><div class="row-actions"><button class="secondary mini" data-edit-contact="${contact.id}">Düzenle</button><button class="danger" data-delete-contact="${contact.id}">Sil</button></div></div>`);
  renderCards("#noteCards", notes, note => `<div class="data-card"><p>${escapeHtml(note.content)}</p><small>${new Date(note.createdAtUtc).toLocaleString("tr-TR")}</small><div class="row-actions"><button class="secondary mini" data-edit-note="${note.id}">Düzenle</button><button class="danger" data-delete-note="${note.id}">Sil</button></div></div>`);
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

const dialog = document.querySelector("#entityDialog");
const entityForm = document.querySelector("#entityForm");

function openForm(title, fields, submit) {
  document.querySelector("#dialogTitle").textContent = title;
  document.querySelector("#dialogFields").innerHTML = fields.map(field => {
    const value = field.value ?? "";
    const required = field.required ? "required" : "";
    const wide = field.wide ? "wide" : "";
    if (field.type === "select") {
      return `<label class="${wide}">${field.label}<select name="${field.name}" ${required}>${field.options.map(option =>
        `<option value="${escapeHtml(String(option.value))}" ${String(option.value) === String(value) ? "selected" : ""}>${escapeHtml(option.label)}</option>`).join("")}</select></label>`;
    }
    if (field.type === "checkbox") {
      return `<label class="${wide} checkbox"><input name="${field.name}" type="checkbox" ${value ? "checked" : ""}> ${field.label}</label>`;
    }
    if (field.type === "textarea") {
      return `<label class="${wide}">${field.label}<textarea name="${field.name}" rows="4" ${required}>${escapeHtml(String(value))}</textarea></label>`;
    }
    return `<label class="${wide}">${field.label}<input name="${field.name}" type="${field.type || "text"}" value="${escapeHtml(String(value))}" ${required}></label>`;
  }).join("");
  dialogSubmit = { fields, submit };
  dialog.showModal();
}

function closeForm() {
  dialog.close();
  dialogSubmit = null;
  entityForm.reset();
}

document.querySelector("#closeDialog").addEventListener("click", closeForm);
document.querySelector("#cancelDialog").addEventListener("click", closeForm);
entityForm.addEventListener("submit", async event => {
  event.preventDefault();
  if (!dialogSubmit) return;
  const data = new FormData(entityForm);
  const values = {};
  for (const field of dialogSubmit.fields) {
    let value = field.type === "checkbox" ? data.has(field.name) : data.get(field.name);
    if (value === "") value = null;
    if (value !== null && field.valueType === "number") value = Number(value);
    if (value !== null && field.valueType === "dateTime") value = new Date(value).toISOString();
    values[field.name] = value;
  }
  try {
    await dialogSubmit.submit(values);
    closeForm();
  } catch (error) { notify(error.message, true); }
});

const textField = (name, label, value = "", required = false, type = "text") => ({ name, label, value, required, type });
const numberField = (name, label, value = 0, required = true) => ({ name, label, value, required, type: "number", valueType: "number" });
const selectField = (name, label, value, options, required = true) => ({
  name, label, value, options, required, type: "select",
  valueType: options.every(option => option.value === "" || Number.isFinite(Number(option.value))) ? "number" : undefined
});
const nullableSelect = (name, label, value, options) => ({ name, label, value: value || "", options: [{ value: "", label: "Seçilmedi" }, ...options], type: "select" });
const customerOptions = () => customers.map(item => ({ value: item.id, label: item.name }));
const stageOptions = () => stages.map(item => ({ value: item.id, label: item.name }));

function openCustomerEditor(customer) {
  openForm("Müşteriyi düzenle", [
    textField("name", "Ad / Şirket", customer.name, true),
    selectField("type", "Tür", customer.type, [{ value: 1, label: "Bireysel" }, { value: 2, label: "Şirket" }]),
    textField("email", "E-posta", customer.email || "", false, "email"), textField("phone", "Telefon", customer.phone),
    textField("website", "Web sitesi", customer.website), textField("industry", "Sektör", customer.industry),
    textField("taxNumber", "Vergi numarası", customer.taxNumber), textField("address", "Adres", customer.address),
    textField("city", "Şehir", customer.city), textField("country", "Ülke", customer.country),
    { name: "isActive", label: "Aktif", value: customer.isActive, type: "checkbox" }
  ], async values => {
    const updated = await api(`/api/customers/${customer.id}`, { method: "PUT", body: JSON.stringify({ ...values, ownerUserId: customer.ownerUserId }) });
    customers = customers.map(item => item.id === updated.id ? updated : item);
    renderAll(); notify("Müşteri güncellendi.");
  });
}

function openActivityEditor(activity) {
  openForm("Aktiviteyi düzenle", [
    textField("subject", "Konu", activity.subject, true), { ...textField("description", "Açıklama", activity.description), type: "textarea", wide: true },
    selectField("type", "Tür", activity.type, [1,2,3,4,5].map((value, index) => ({ value, label: ["Arama","E-posta","Toplantı","Görev","Demo"][index] }))),
    selectField("status", "Durum", activity.status, [{ value: 1, label: "Planlandı" }, { value: 2, label: "Tamamlandı" }, { value: 3, label: "İptal" }]),
    { ...textField("dueAtUtc", "Bitiş", activity.dueAtUtc?.slice(0,16), false, "datetime-local"), valueType: "dateTime" }
  ], async values => {
    const payload = { ...values, assignedUserId: activity.assignedUserId, completedAtUtc: activity.completedAtUtc };
    const updated = await api(`/api/activities/${activity.id}`, { method: "PUT", body: JSON.stringify(payload) });
    activities = activities.map(item => item.id === updated.id ? updated : item);
    renderAll(); notify("Aktivite güncellendi.");
  });
}

function openLeadEditor(lead = null) {
  const editing = Boolean(lead);
  openForm(editing ? "Lead düzenle" : "Yeni lead", [
    textField("firstName", "Ad", lead?.firstName, true), textField("lastName", "Soyad", lead?.lastName, true),
    textField("companyName", "Şirket", lead?.companyName), textField("email", "E-posta", lead?.email, false, "email"),
    textField("phone", "Telefon", lead?.phone), textField("source", "Kaynak", lead?.source),
    ...(editing ? [selectField("status", "Durum", lead.status, [1,2,3,4,5,6].map(value => ({ value, label: ["Yeni","İletişimde","Nitelikli","Niteliksiz","Dönüştürüldü","Kaybedildi"][value-1] }))), numberField("score", "Skor", lead.score)] : [])
  ], async values => {
    const payload = { ...values, ownerUserId: lead?.ownerUserId || null };
    const saved = await api(editing ? `/api/leads/${lead.id}` : "/api/leads", { method: editing ? "PUT" : "POST", body: JSON.stringify(payload) });
    leads = editing ? leads.map(item => item.id === saved.id ? saved : item) : [saved, ...leads];
    renderExtended(); notify(editing ? "Lead güncellendi." : "Lead eklendi.");
  });
}

function openOpportunityEditor(opportunity = null) {
  if (!customers.length || !stages.length) { notify("Önce müşteri ve pipeline aşaması oluşturun.", true); return; }
  const editing = Boolean(opportunity);
  openForm(editing ? "Fırsatı düzenle" : "Yeni fırsat", [
    textField("title", "Başlık", opportunity?.title, true),
    ...(editing ? [] : [selectField("customerId", "Müşteri", customers[0].id, customerOptions())]),
    selectField("pipelineStageId", "Aşama", opportunity?.pipelineStageId || stages[0].id, stageOptions()),
    numberField("amount", "Tutar", opportunity?.amount || 0), textField("currency", "Para birimi", opportunity?.currency || "TRY", true),
    numberField("probability", "Olasılık", opportunity?.probability || 0),
    { ...textField("expectedCloseDate", "Tahmini kapanış", opportunity?.expectedCloseDate, false, "date") },
    ...(editing ? [selectField("status", "Durum", opportunity.status, [{ value: 1, label: "Açık" }, { value: 2, label: "Kazanıldı" }, { value: 3, label: "Kaybedildi" }]), textField("lostReason", "Kaybetme nedeni", opportunity.lostReason)] : [])
  ], async values => {
    const payload = { ...values, contactId: opportunity?.contactId || null, ownerUserId: opportunity?.ownerUserId || null };
    const saved = await api(editing ? `/api/opportunities/${opportunity.id}` : "/api/opportunities", { method: editing ? "PUT" : "POST", body: JSON.stringify(payload) });
    opportunities = editing ? opportunities.map(item => item.id === saved.id ? saved : item) : [saved, ...opportunities];
    renderExtended(); notify(editing ? "Fırsat güncellendi." : "Fırsat eklendi.");
  });
}

async function openLeadConverter(lead) {
  const contactGroups = await Promise.all(customers.map(async customer => ({
    customer,
    contacts: await safeApi(`/api/contacts/customer/${customer.id}`, [])
  })));
  const pairs = contactGroups.flatMap(group => group.contacts.map(contact => ({
    value: `${group.customer.id}|${contact.id}`,
    label: `${group.customer.name} · ${contact.firstName} ${contact.lastName}`
  })));
  if (!pairs.length) { notify("Lead dönüşümü için müşteriye bağlı en az bir kişi gerekli.", true); return; }
  openForm("Lead'i dönüştür", [
    { name: "pair", label: "Müşteri ve kişi", value: pairs[0].value, type: "select", required: true, options: pairs }
  ], async values => {
    const [customerId, contactId] = values.pair.split("|");
    const updated = await api(`/api/leads/${lead.id}/convert`, { method: "POST", body: JSON.stringify({ customerId, contactId }) });
    leads = leads.map(item => item.id === updated.id ? updated : item);
    renderExtended(); notify("Lead müşteriye dönüştürüldü.");
  });
}

function openStageChanger(opportunity) {
  openForm("Fırsat aşamasını değiştir", [
    selectField("stageId", "Pipeline aşaması", opportunity.pipelineStageId, stageOptions())
  ], async values => {
    const updated = await api(`/api/opportunities/${opportunity.id}/stage/${values.stageId}`, { method: "PATCH" });
    opportunities = opportunities.map(item => item.id === updated.id ? updated : item);
    renderExtended(); notify("Fırsat aşaması güncellendi.");
  });
}

document.querySelector("#newLead").addEventListener("click", () => openLeadEditor());
document.querySelector("#newOpportunity").addEventListener("click", () => openOpportunityEditor());

function selectedCustomerId() {
  return document.querySelector("#relatedCustomer").value;
}

async function reloadRelations() {
  document.querySelector("#relatedCustomer").dispatchEvent(new Event("change"));
}

function openContactEditor(contact = null) {
  const customerId = contact?.customerId || selectedCustomerId();
  if (!customerId) { notify("Önce müşteri seçin.", true); return; }
  const editing = Boolean(contact);
  openForm(editing ? "Kişiyi düzenle" : "Yeni kişi", [
    textField("firstName", "Ad", contact?.firstName, true), textField("lastName", "Soyad", contact?.lastName, true),
    textField("jobTitle", "Görev", contact?.jobTitle), textField("email", "E-posta", contact?.email, false, "email"),
    textField("phone", "Telefon", contact?.phone),
    { name: "isPrimary", label: "Birincil kişi", value: contact?.isPrimary, type: "checkbox" },
    { name: "hasEmailConsent", label: "E-posta izni", value: contact?.hasEmailConsent, type: "checkbox" }
  ], async values => {
    await api(editing ? `/api/contacts/${contact.id}` : "/api/contacts", {
      method: editing ? "PUT" : "POST", body: JSON.stringify(editing ? values : { customerId, ...values })
    });
    await reloadRelations(); notify(editing ? "Kişi güncellendi." : "Kişi eklendi.");
  });
}

function openNoteEditor(note = null) {
  const customerId = note?.customerId || selectedCustomerId();
  if (!customerId) { notify("Önce müşteri seçin.", true); return; }
  const editing = Boolean(note);
  openForm(editing ? "Notu düzenle" : "Yeni not", [
    { name: "content", label: "Not", value: note?.content, required: true, type: "textarea", wide: true }
  ], async values => {
    await api(editing ? `/api/notes/${note.id}` : "/api/notes", {
      method: editing ? "PUT" : "POST",
      body: JSON.stringify(editing ? values : { ...values, customerId, contactId: null, leadId: null, opportunityId: null })
    });
    await reloadRelations(); notify(editing ? "Not güncellendi." : "Not eklendi.");
  });
}

function openStageEditor(stage = null) {
  const editing = Boolean(stage);
  openForm(editing ? "Aşamayı düzenle" : "Yeni pipeline aşaması", [
    textField("name", "Ad", stage?.name, true), numberField("order", "Sıra", stage?.order || stages.length + 1),
    numberField("winProbability", "Kazanma olasılığı", stage?.winProbability || 0),
    textField("color", "Renk", stage?.color || "#7c5cff", true, "color"),
    ...(editing ? [{ name: "isActive", label: "Aktif", value: stage.isActive, type: "checkbox" }] : [])
  ], async values => {
    const saved = await api(editing ? `/api/pipeline-stages/${stage.id}` : "/api/pipeline-stages", {
      method: editing ? "PUT" : "POST", body: JSON.stringify(values)
    });
    stages = editing ? stages.map(item => item.id === saved.id ? saved : item) : [...stages, saved];
    renderExtended(); notify(editing ? "Aşama güncellendi." : "Aşama eklendi.");
  });
}

function openInsightCreator() {
  openForm("Yeni AI içgörüsü", [
    selectField("type", "Tür", 1, [1,2,3,4,5,6].map((value, index) => ({ value, label: ["Lead skoru","Müşteri kaybı riski","Sonraki aksiyon","Duygu","Fırsat tahmini","Özet"][index] }))),
    textField("title", "Başlık", "", true), { name: "content", label: "İçerik", required: true, type: "textarea", wide: true },
    numberField("score", "Skor", 0, false), numberField("confidence", "Güven", 0, false),
    nullableSelect("customerId", "Müşteri", "", customerOptions()), textField("model", "Model", "manual", true)
  ], async values => {
    const saved = await api("/api/ai-insights", { method: "POST", body: JSON.stringify({ ...values, leadId: null, opportunityId: null, expiresAtUtc: null }) });
    insights = [saved, ...insights]; renderExtended(); notify("AI içgörüsü eklendi.");
  });
}

function openUserEditor(user = null) {
  const editing = Boolean(user);
  openForm(editing ? "Kullanıcıyı düzenle" : "Yeni kullanıcı", [
    ...(!editing ? [textField("email", "E-posta", "", true, "email")] : []),
    textField("fullName", "Ad soyad", user?.fullName, true),
    ...(!editing ? [textField("password", "Parola", "", true, "password")] : []),
    { name: "role", label: "Rol", value: user?.role || "User", type: "select", required: true, options: [{ value: "Admin", label: "Admin" }, { value: "User", label: "User" }] },
    ...(editing ? [{ name: "isActive", label: "Aktif", value: user.isActive, type: "checkbox" }] : [])
  ], async values => {
    const saved = await api(editing ? `/api/users/${user.id}` : "/api/users", { method: editing ? "PUT" : "POST", body: JSON.stringify(values) });
    users = editing ? users.map(item => item.id === saved.id ? saved : item) : [saved, ...users];
    renderExtended(); notify(editing ? "Kullanıcı güncellendi." : "Kullanıcı eklendi.");
  });
}

function openTenantEditor() {
  openForm("Tenant ayarları", [
    textField("name", "Ad", tenant.name, true), textField("logoUrl", "Logo URL", tenant.logoUrl),
    textField("timeZone", "Saat dilimi", tenant.timeZone, true), textField("currency", "Para birimi", tenant.currency, true),
    { name: "isActive", label: "Aktif", value: tenant.isActive, type: "checkbox" }
  ], async values => {
    tenant = await api(`/api/tenants/${tenant.id}`, { method: "PUT", body: JSON.stringify(values) });
    renderExtended(); notify("Tenant güncellendi.");
  });
}

function openSubscriptionEditor() {
  const editing = Boolean(subscription);
  const now = new Date();
  const nextYear = new Date(now); nextYear.setFullYear(now.getFullYear() + 1);
  openForm(editing ? "Aboneliği düzenle" : "Abonelik oluştur", [
    textField("planCode", "Plan kodu", subscription?.planCode || "PRO", true),
    ...(editing ? [selectField("status", "Durum", subscription.status, [1,2,3,4,5].map(value => ({ value, label: ["Deneme","Aktif","Gecikmiş","İptal","Süresi doldu"][value-1] })))] : []),
    numberField("seatLimit", "Kullanıcı limiti", subscription?.seatLimit || 10),
    { ...textField("periodStartUtc", "Başlangıç", (subscription?.periodStartUtc || now.toISOString()).slice(0,16), true, "datetime-local"), valueType: "dateTime" },
    { ...textField("periodEndUtc", "Bitiş", (subscription?.periodEndUtc || nextYear.toISOString()).slice(0,16), true, "datetime-local"), valueType: "dateTime" },
    ...(editing ? [{ name: "cancelAtPeriodEnd", label: "Dönem sonunda iptal", value: subscription.cancelAtPeriodEnd, type: "checkbox" }] : [])
  ], async values => {
    subscription = await api(editing ? `/api/subscriptions/${subscription.id}` : "/api/subscriptions", {
      method: editing ? "PUT" : "POST", body: JSON.stringify(values)
    });
    renderExtended(); notify(editing ? "Abonelik güncellendi." : "Abonelik oluşturuldu.");
  });
}

document.querySelector("#newContact").addEventListener("click", () => openContactEditor());
document.querySelector("#newNote").addEventListener("click", () => openNoteEditor());
document.querySelector("#newStage").addEventListener("click", () => openStageEditor());
document.querySelector("#newInsight").addEventListener("click", openInsightCreator);
document.querySelector("#newUser").addEventListener("click", () => openUserEditor());

document.querySelector("#geminiForm").addEventListener("submit", async event => {
  event.preventDefault();
  const button = document.querySelector("#geminiSubmit");
  const result = document.querySelector("#geminiResult");
  const context = JSON.stringify({
    customerCount: customers.length,
    activityCount: activities.length,
    leadCount: leads.length,
    opportunityCount: opportunities.length,
    customers: customers.slice(0, 25).map(item => ({ name: item.name, industry: item.industry, city: item.city })),
    leads: leads.slice(0, 25).map(item => ({ name: `${item.firstName} ${item.lastName}`, company: item.companyName, status: item.status, score: item.score })),
    opportunities: opportunities.slice(0, 25).map(item => ({ title: item.title, amount: item.amount, currency: item.currency, status: item.status, probability: item.probability })),
    activities: activities.slice(0, 25).map(item => ({ subject: item.subject, status: item.status, dueAtUtc: item.dueAtUtc }))
  });

  button.disabled = true;
  button.textContent = "Gemini düşünüyor…";
  result.classList.add("hidden");
  try {
    const response = await api("/api/ai-assistant/generate", {
      method: "POST",
      body: JSON.stringify({ prompt: document.querySelector("#geminiPrompt").value, context })
    });
    result.textContent = response.text;
    result.classList.remove("hidden");
  } catch (error) {
    notify(error.message, true);
  } finally {
    button.disabled = false;
    button.textContent = "Analiz oluştur";
  }
});

document.addEventListener("click", async event => {
  const id = name => event.target.dataset[name];
  try {
    if (id("editCustomer")) return openCustomerEditor(customers.find(item => item.id === id("editCustomer")));
    if (id("editActivity")) return openActivityEditor(activities.find(item => item.id === id("editActivity")));
    if (id("editLead")) return openLeadEditor(leads.find(item => item.id === id("editLead")));
    if (id("convertLead")) return openLeadConverter(leads.find(item => item.id === id("convertLead")));
    if (id("editOpportunity")) return openOpportunityEditor(opportunities.find(item => item.id === id("editOpportunity")));
    if (id("stageOpportunity")) return openStageChanger(opportunities.find(item => item.id === id("stageOpportunity")));
    if (id("editContact")) return openContactEditor(selectedContacts.find(item => item.id === id("editContact")));
    if (id("editNote")) return openNoteEditor(selectedNotes.find(item => item.id === id("editNote")));
    if (id("editStage")) return openStageEditor(stages.find(item => item.id === id("editStage")));
    if (id("editUser")) return openUserEditor(users.find(item => item.id === id("editUser")));
    if (id("editTenant")) return openTenantEditor();
    if (id("editSubscription") || event.target.id === "newSubscription") return openSubscriptionEditor();
    if (id("deleteLead")) { await remove(`/api/leads/${id("deleteLead")}`, "Lead"); leads = leads.filter(item => item.id !== id("deleteLead")); }
    else if (id("deleteOpportunity")) { await remove(`/api/opportunities/${id("deleteOpportunity")}`, "Fırsat"); opportunities = opportunities.filter(item => item.id !== id("deleteOpportunity")); }
    else if (id("deleteContact")) { await remove(`/api/contacts/${id("deleteContact")}`, "Kişi"); return reloadRelations(); }
    else if (id("deleteNote")) { await remove(`/api/notes/${id("deleteNote")}`, "Not"); return reloadRelations(); }
    else if (id("deleteStage")) { await remove(`/api/pipeline-stages/${id("deleteStage")}`, "Aşama"); stages = stages.filter(item => item.id !== id("deleteStage")); }
    else if (id("deleteInsight")) { await remove(`/api/ai-insights/${id("deleteInsight")}`, "İçgörü"); insights = insights.filter(item => item.id !== id("deleteInsight")); }
    else if (id("deleteUser")) { await remove(`/api/users/${id("deleteUser")}`, "Kullanıcı"); users = users.filter(item => item.id !== id("deleteUser")); }
    else if (id("dismissInsight")) {
      const insight = insights.find(item => item.id === id("dismissInsight"));
      const updated = await api(`/api/ai-insights/${insight.id}/dismiss`, { method: "PATCH", body: JSON.stringify({ isDismissed: !insight.isDismissed }) });
      insights = insights.map(item => item.id === updated.id ? updated : item);
    } else if (id("cancelSubscription")) {
      if (!confirm("Abonelik dönem sonunda iptal edilsin mi?")) return;
      subscription = await api(`/api/subscriptions/${id("cancelSubscription")}/cancel?atPeriodEnd=true`, { method: "POST" });
      notify("Abonelik iptal için işaretlendi.");
    } else return;
    renderExtended();
  } catch (error) { notify(error.message, true); }
});

async function remove(path, label) {
  if (!confirm(`${label} silinsin mi?`)) throw new Error("İşlem iptal edildi.");
  await api(path, { method: "DELETE" });
  notify(`${label} silindi.`);
}
