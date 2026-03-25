<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
// TODO: página bugada
const presentes = ref([]);

const carregarPresentes = async () => {
    const response = await api.get('/chas/1/presentes'); // Use o ID real do chá
    presentes.value = response.data;
};

onMounted(carregarPresentes);
</script>

<template>
    <div class="card">
        <DataView :value="presentes" layout="grid">
            <template #grid="slotProps">
                <div class="col-12 md:col-4 p-2">
                    <Card>
                        <template #header>
                            <img :src="`http://localhost:5000/api/presentes/${slotProps.data.id}/imagem`" 
                                 alt="foto do presente" class="w-full" />
                        </template>
                        <template #title> {{ slotProps.data.nome }} </template>
                        <template #content>
                            <p>Restam: {{ slotProps.data.quantidadeRestante }}</p>
                        </template>
                        <template #footer>
                            <Button label="Reservar" icon="pi pi-check" 
                                    :disabled="slotProps.data.estaEsgotado" />
                        </template>
                    </Card>
                </div>
            </template>
        </DataView>
    </div>
</template>