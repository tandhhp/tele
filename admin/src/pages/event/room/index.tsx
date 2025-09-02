import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { useRef, useState } from "react";
import RoomForm from "./components/form";
import { Button } from "antd";
import { PlusOutlined } from "@ant-design/icons";

const Index: React.FC = () => {

    const actionRef = useRef<ActionType>(null);
    const [openForm, setOpenForm] = useState<boolean>(false);
    const [selectedRoom, setSelectedRoom] = useState<any>(null);

    return (
        <PageContainer extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => setOpenForm(true)}>Thêm phòng</Button>}>
            <ProTable
                search={{
                    layout: 'vertical'
                }}
                rowKey="id"
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Tên phòng',
                        dataIndex: 'name',
                    },
                    {
                        title: 'Số bàn',
                        dataIndex: 'tableCount',
                    },
                    {
                        title: 'Trạng thái',
                        dataIndex: 'status',
                    },
                ]}
            />
            <RoomForm open={openForm} onOpenChange={setOpenForm} reload={() => actionRef.current?.reload()} data={selectedRoom} />
        </PageContainer>
    )
}

export default Index;