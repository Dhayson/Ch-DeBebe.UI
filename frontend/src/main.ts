import './assets/main.css'
import { createApp } from 'vue'
import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import ToastService from 'primevue/toastservice';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';
import Button from 'primevue/button';
import Toast from 'primevue/toast';
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
// Global registration of frequently used PrimeVue components
app.component('Card', Card);
app.component('InputText', InputText);
app.component('Password', Password);
app.component('Button', Button);
app.component('Toast', Toast);

app.mount("#app");
