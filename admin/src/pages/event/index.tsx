import { apiAddEvent, apiUpdateEvent } from "@/services/contact";
import { DeleteOutlined, EditOutlined, FolderOutlined } from "@ant-design/icons";
import { ActionType, ModalForm, PageContainer, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText, ProTable } from "@ant-design/pro-components"
import { Link, request } from "@umijs/max";
import { Button, Col, message, Row } from "antd";
import { useEffect, useRef, useState } from "react";

const EventPage: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const [event, setEvent] = useState<any>();
    const formRef = useRef<ProFormInstance>();

    useEffect(() => {
        if (event) {
            formRef.current?.setFields([
                {
                    name: 'id',
                    value: event.id
                },
                {
                    name: 'name',
                    value: event.name
                },
                {
                    name: 'startDate',
                    value: event.startDate
                },
                {
                    name: 'time',
                    value: event.time
                }
            ])
        }
    }, [event]);

    const onFinish = async (values: any) => {
        if (values.id) {
            await apiUpdateEvent(values);
        } else {
            await apiAddEvent(values);
        }
        message.success('Thành công!');
        setOpen(false);
        actionRef.current?.reload();
    }

    return (
        <PageContainer>
            <ProTable
                scroll={{
                    x: true
                }}
                actionRef={actionRef}
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 40
                    },
                    {
                        title: 'Tên sự kiện',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Ngày diễn ra',
                        dataIndex: 'startDate',
                        valueType: 'date',
                        search: false,
                    },
                    {
                        title: 'Thời gian',
                        dataIndex: 'time',
                        search: false
                    },
                    {
                        title: 'Thành phần tham gia',
                        valueType: 'option',
                        render: (_, entity) => [
                            <Link to={`/event/user/${entity.id}`} key="detail">
                                <Button type="primary" size="small" icon={<FolderOutlined />}>Chi tiết</Button>
                            </Link>
                        ],
                        width: 200
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, entity) => [
                            <Button key="edit" icon={<EditOutlined />} size="small" onClick={() => {
                                setEvent(entity);
                                setOpen(true);
                            }}></Button>,
                            <Button key="de" icon={<DeleteOutlined />} size="small" type="primary" danger></Button>
                        ],
                        width: 100
                    }
                ]}
                request={(params) => request(`contact/event/list`, { params })}
            />
            <ModalForm title="Sự kiện" open={open} onOpenChange={setOpen} onFinish={onFinish} formRef={formRef}>
                <ProFormText name="id" hidden />
                <ProFormText name="name" label="Tên sự kiện" rules={[
                    {
                        required: true
                    }
                ]} />
                <Row gutter={16}>
                    <Col md={12}>
                        <ProFormDatePicker name="startDate" label="Ngày diễn ra" className="w-full" width={"lg"} rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                    <Col md={12}>
                        <ProFormSelect name="time" label="Thời gian" options={["09:00", "14:30"]} rules={[
                            {
                                required: true
                            }
                        ]} />
                    </Col>
                </Row>
            </ModalForm>
        </PageContainer>
    )
}

export default EventPage;