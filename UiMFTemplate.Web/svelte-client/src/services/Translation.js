// eslint-disable-next-line import/no-extraneous-dependencies
import { get, derived, writable } from "svelte/store";
// eslint-disable-next-line import/no-extraneous-dependencies
import moment from "moment/min/moment-with-locales";
import {
  _,
  init,
  locale,
  dictionary,
  addMessages,
} from "svelte-i18n";

const MESSAGE_FILE_URL_TEMPLATE = "/UiMFForm/Locale?language={locale}";

// eslint-disable-next-line no-underscore-dangle
let _activeLocale;

const isDownloading = writable(false);

const momentLocale = writable(
  moment.locale("en-us", {
    postformat(t) {
      return t;
    },
  }),
);

const isLocaleLoaded = derived(
  [isDownloading, dictionary],
  // eslint-disable-next-line no-underscore-dangle
  ([$isDownloading, $dictionary]) => !$isDownloading && $dictionary[_activeLocale]
    // eslint-disable-next-line no-underscore-dangle
    && Object.keys($dictionary[_activeLocale]).length > 0,
);

const dir = derived(locale, ($locale) => ($locale === "ar" ? "rtl" : "ltr"));

function loadJson(url) {
  return fetch(url).then((response) => {
    return response.json();
  });
}

function hasLoadedLocale(l) {
  return get(dictionary)[l];
}

function setMomentLocale(l) {
  if (l === "en") {
    moment.updateLocale("en-us", {
      postformat(t) {
        return t;
      },
    });
  } else {
    moment.updateLocale("ar", {
      postformat(t) {
        return t;
      },
    });
  }
  momentLocale.set(moment);
}


function initI18n({ withLocale: _locale } = { withLocale: "en" }) {
  init({ initialLocale: _locale, warnOnMissingMessages: false });
  setMomentLocale(_locale);
}

function setupI18n({ withLocale: _locale } = { withLocale: "en" }) {
  initI18n(_locale);
  if (!hasLoadedLocale(_locale) && !(get(isDownloading))) {
    isDownloading.set(true);
    const messagesFileUrl = MESSAGE_FILE_URL_TEMPLATE.replace(
      "{locale}",
      _locale,
    );
    return loadJson(messagesFileUrl).then((messages) => {
      _activeLocale = _locale;
      
      addMessages(_locale, messages);

      locale.set(_locale);

      isDownloading.set(false);
    });
  }
  return null;
}

export {
  _, setupI18n, isLocaleLoaded, locale, dir, momentLocale, initI18n
};
