<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useConfirm } from "primevue/useconfirm";

const router = useRouter();
const confirm = useConfirm();

const items = ref([
    {
        label: 'Dashboard',
        icon: 'pi pi-home',
        command: () => router.push('/dashboard')
    },
    {
        label: 'Meus Eventos',
        icon: 'pi pi-calendar',
        command: () => router.push('/chas_inscrito')
    }
]);

const handleLogout = () => {
    confirm.require({
        message: 'Você tem certeza que deseja sair do sistema?',
        header: 'Confirmação de Saída',
        icon: 'pi pi-exclamation-triangle',
        acceptLabel: 'Sim, sair',
        rejectLabel: 'Cancelar',
        acceptClass: 'p-button-danger', // Deixa o botão de confirmar vermelho
        accept: () => {
            // Só executa se o usuário clicar em "Sim"
            localStorage.removeItem('token');
            router.push('/login');
        },
        reject: () => {
            // Opcional: ação ao cancelar (geralmente vazio)
        }
    });
};
</script>

<template>
    <Menubar 
        :model="items" 
        class="w-full border-none border-noround shadow-2 px-4 fixed top-0 left-0 z-5"
    >
        <template #start>
            <div class="flex align-items-center mr-4">
                <i class="pi pi-heart-fill text-primary text-2xl mr-2"></i>
                <span class="font-bold text-xl text-primary">ChaDeBebe.ui</span>
            </div>
        </template>

        <template #end>
            <Button 
                label="Sair" 
                icon="pi pi-sign-out" 
                severity="danger" 
                text 
                @click="handleLogout" 
            />
        </template>
    </Menubar>
</template>

<style scoped>
/* Forçamos o componente a ignorar qualquer max-width que o 
   tema do PrimeVue possa ter aplicado internamente.
*/
:deep(.p-menubar) {
    width: 100vw !important;
    max-width: 100vw !important;
}
</style>