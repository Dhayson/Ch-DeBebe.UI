
<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api'; // O serviço axios que configuramos
import { useToast } from 'primevue/usetoast';

const router = useRouter();
const toast = useToast();

const loading = ref(false);
const email = ref('');
const password = ref('');

const handleLogin = async () => {
    console.log("Tentando login com:", email.value);
    if (!email.value || !password.value) {
        toast.add({ severity: 'warn', summary: 'Atenção', detail: 'Preencha todos os campos', life: 3000 });
        return;
    }

    loading.value = true;
    try {
        // Envia para o seu backend .NET
        const response = await api.post('http://localhost:5000/api/auth/login', {
            email: email.value,
            senha: password.value
        });

        // Guarda o JWT que o C# gerou
        localStorage.setItem('token', response.data.token);
        
        toast.add({ severity: 'success', summary: 'Login realizado', detail: 'Bem-vindo de volta!', life: 3000 });
        
        // Redireciona para a lista de chás
        router.push('/dashboard'); 
    } catch (error) {
        const mensagem = error.response?.data?.message || 'E-mail ou senha inválidos';
        toast.add({ severity: 'error', summary: 'Falha no Login', detail: mensagem, life: 5000 });
    } finally {
        loading.value = false;
    }
    loading.value = false;
};
</script>

<template>
    <div class="flex align-items-center justify-content-center min-h-screen surface-ground p-4">
        <Card style="width: 100%; max-width: 400px;" class="shadow-4">
            <template #title>
                <div class="text-center">
                    <i class="pi pi-user-circle text-primary text-5xl mb-3"></i>
                    <h2 class="m-0">Acesse sua Conta</h2>
                </div>
            </template>
            
            <template #content>
                <form @submit.prevent="handleLogin" class="flex flex-column gap-4">
                    <div class="flex flex-column gap-2">
                        <label for="email" class="font-bold">E-mail</label>
                        <InputText 
                            id="email" 
                            v-model="email" 
                            type="string" 
                            placeholder="exemplo@email.com" 
                            class="w-full"
                        />
                    </div>

                    <div class="flex flex-column gap-2">
                        <label for="password" class="font-bold">Senha</label>
                        <Password 
                            id="password" 
                            v-model="password" 
                            :feedback="false" 
                            toggleMask 
                            class="w-full"
                            inputClass="w-full"
                            placeholder="Sua senha"
                        />
                    </div>

                    <Button 
                        type="submit" 
                        label="Entrar" 
                        icon="pi pi-sign-in" 
                        :loading="loading" 
                        class="w-full p-button-lg"
                    />
                </form>
            </template>

            <template #footer>
                <div class="text-center text-sm text-600">
                    Ainda não tem conta? 
                    <router-link to="/cadastro" class="text-primary no-underline font-bold">Crie uma agora</router-link>
                </div>
            </template>
        </Card>
    </div>
    
    <Toast />
</template>

<style scoped>
/* Garante que o input de senha ocupe toda a largura do componente Password */
:deep(.p-password input) {
    width: 100%;
}
</style>