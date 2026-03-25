import './assets/main.css'
import 'primeflex/primeflex.css';
import 'primeicons/primeicons.css';

import { createApp } from 'vue'
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';

import ToastService from 'primevue/toastservice';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Calendar from 'primevue/calendar';
import ProgressSpinner from 'primevue/progressspinner';
import Tooltip from 'primevue/tooltip';
import Password from 'primevue/password';
import Button from 'primevue/button';
import Toast from 'primevue/toast';
import Menubar from 'primevue/menubar';
import ConfirmationService from 'primevue/confirmationservice';
import ConfirmDialog from 'primevue/confirmdialog';
import Dialog from 'primevue/dialog'
import Tag from 'primevue/tag';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputNumber from 'primevue/inputnumber';
import Textarea from 'primevue/textarea';

import App from "./App.vue"
import router from './router';

const app = createApp(App);
app.use(PrimeVue, {
    theme: {
        preset: Aura,
        ripple: true,
        inputVariant: "filled",
        options: {
            prefix: 'p',
            darkModeSelector: 'system',
            cssLayer: false
        }
    }
});
app.use(ToastService);
app.use(router);
app.use(ConfirmationService);
app.component('ConfirmDialog', ConfirmDialog);
// Global registration of frequently used PrimeVue components
app.component('Menubar', Menubar);
app.component('Card', Card);
app.component('Calendar', Calendar);
app.component('InputText', InputText);
app.component('Password', Password);
app.component('Button', Button);
app.component('Toast', Toast);
app.component('ProgressSpinner', ProgressSpinner);
app.directive('tooltip', Tooltip);
app.component('Tag', Tag);
app.component('Dialog', Dialog);
app.component('DataTable', DataTable);
app.component('Column', Column);
app.component('InputNumber', InputNumber);
app.component('Textarea', Textarea);

app.mount("#app");
